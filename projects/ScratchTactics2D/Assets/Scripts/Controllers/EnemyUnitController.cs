using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Extensions;

public class EnemyUnitController : Controller
{
	private bool subjectsActingTrigger;
	private TacticsGrid grid;
	
	protected void Awake() {
		base.Awake();
		myPhase = Enum.Phase.enemy;
		grid = GameManager.inst.tacticsManager.GetActiveGrid();
		subjectsActingTrigger = false;
	}

	public override bool MyPhaseActive() {
		return GameManager.inst.phaseManager.currentPhase == myPhase && GameManager.inst.gameState == Enum.GameState.battle;
	}

	public override void TriggerPhase() {
		phaseActionState = Enum.PhaseActionState.waitingForInput;
		//
		subjectsActingTrigger = true;
	}

	public override void Register(MovingObject subject) {
		base.Register(subject);
		//
		Unit unit = subject as Unit;
		unit.parentController = this;
	}
	
	void Update() {
		if (!MyPhaseActive()) return;
		
		switch(phaseActionState) {
			case Enum.PhaseActionState.waitingForInput:
				// start action coroutine if not currently running
				// reset trigger immediately
				if (subjectsActingTrigger) {
					subjectsActingTrigger = false;
					phaseActionState = Enum.PhaseActionState.acting;
					
					StartCoroutine(SubjectsTakeAction());
				}
				break;
				
			case Enum.PhaseActionState.acting:
				// do nothing until finished acting
				bool endPhaseNow = true;
				foreach (Unit unit in activeRegistry) {
					if (unit.AnyOptionActive()) {
						endPhaseNow = false;
						break;
					}
				}
				if (endPhaseNow) phaseActionState = Enum.PhaseActionState.complete;
				break;
				
			case Enum.PhaseActionState.complete:
				phaseActionState = Enum.PhaseActionState.postPhaseDelay;
				RefreshAllUnits();
				EndPhase();
				break;
				
			// delay for phaseDelayTime, until you go into postPhase
			case Enum.PhaseActionState.postPhaseDelay:
			case Enum.PhaseActionState.postPhase:
				break;
		}
	}

	public IEnumerator SubjectsTakeAction() {
		// cache the playerController to pull from its registry
		var playerController = GameManager.inst.tacticsManager.activeBattle.GetControllerFromPhase(Enum.Phase.player);

		for (int i = 0; i < activeRegistry.Count; i++) {
			Unit subject = (Unit)activeRegistry[i];
			var grid = GameManager.inst.tacticsManager.GetActiveGrid();
			
			// determine some order in which the enemies act
			// target selection
			// if outside of range, just move towards the closest adversary
			// if multiple adversaries in range, pick the adversary to which you can do the most damage
			subject.UpdateThreatRange();

			var targetPosition = GetTargetInAttackRange(subject, playerController.activeRegistry);
			if (targetPosition == -1*Vector3Int.one) targetPosition = GetClosestTarget(subject, playerController.activeRegistry);

			// now that we have the target location, find all the "attackable" squares surrounding it
			// then, select the optimal square to move into to perform your attack
			var optimalPosition = GetOptimalToAttack(subject, targetPosition);

			if (subject.OptionActive("Move") && optimalPosition != subject.gridPosition) {
				// now, find a full path to the location
				// even if we can't reach it, just go as far as you can
				var pathToTarget = MovingObjectPath.GetPathTo(subject.gridPosition, optimalPosition, subject.obstacles);
				var clippedPath  = MovingObjectPath.Clip(pathToTarget, subject.moveRange);
				//
				subject.TraverseTo(clippedPath.end, fieldPath: clippedPath);
				subject.SetOption("Move", false);

				// spin until this subject is entirely done moving
				while (subject.IsMoving()) { yield return null; }
			}

			if (subject.OptionActive("Attack") && subject.attackRange.ValidAttack(subject, targetPosition)) {
				var unitAt = (Unit)grid.OccupantAt(targetPosition);
				subject.Attack(unitAt);
				//
				subject.SetOption("Attack", false);
			}

			// finally:
			// this will discolor the unit and set its options to false, after movement is complete
			// BUT, don't let the other units move until this subject has finished
			subject.OnEndTurn();

			// don't delay if you're the last
			if (i != activeRegistry.Count-1) {
				yield return new WaitForSeconds(0.5f);
			}
		}
	}

	private Vector3Int GetTargetInAttackRange(Unit subject, List<MovingObject> targets) {
		var attackTargets = targets.FindAll(it => subject.attackRange.field.ContainsKey(it.gridPosition));
		return (attackTargets.Any()) ? GetClosestTarget(subject, attackTargets) : -1*Vector3Int.one;
	}

	private Vector3Int GetClosestTarget(Unit subject, List<MovingObject> targets) {
		var minDist = targets.Min(it => (subject.gridPosition - it.gridPosition).magnitude);
		return targets.First(it => (subject.gridPosition - it.gridPosition).magnitude == minDist).gridPosition;
	}

	private Vector3Int GetOptimalToAttack(Unit subject, Vector3Int targetPosition) {
		bool Traversable(Vector3Int v) { return grid.IsInBounds(v) && (grid.VacantAt(v) || v == subject.gridPosition); }
		float DistToTarget(Vector3Int v) { return targetPosition.ManhattanDistance(v); }
		float DistToSubject(Vector3Int v) { return subject.gridPosition.ManhattanDistance(v); }
		// util

		// max allowable attack positions (max range/reach)
		var targetable = targetPosition.Radiate(subject.attackReach).Where(it => Traversable(it));
		float maxDistWithin = targetable.Max(it => DistToTarget(it));
		var atMaxDist = targetable.Where(it => DistToTarget(it) == maxDistWithin);

		// the closest of those to the acting subject
		float minDistSubject = atMaxDist.Min(it => DistToSubject(it));
		var optimalPosition = atMaxDist.First(it => DistToSubject(it) == minDistSubject);
		return optimalPosition;
	}

	private Vector3Int GetControllerCentroid(Controller controller) {
		Vector3 _oppositionCentroid = Vector3.zero;
		foreach (var adv in controller.activeRegistry) {
			_oppositionCentroid += adv.gridPosition;
		}
		_oppositionCentroid /= (controller.activeRegistry.Count);
		Vector3Int oppositionCentroid = new Vector3Int(Mathf.RoundToInt(_oppositionCentroid.x),
													   Mathf.RoundToInt(_oppositionCentroid.y),
													   Mathf.RoundToInt(_oppositionCentroid.z));
		return oppositionCentroid;
	}
}