using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Extensions;

public class EnemyUnitController : UnitController
{
	private bool subjectsActingTrigger;
	private TacticsGrid grid;
	
	protected override void Awake() {
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
		activeRegistry.ForEach(u => ((Unit)u).OnStartTurn());
		subjectsActingTrigger = true;
	}

	public override void Register(MovingObject subject) {
		base.Register(subject);
		//
		Unit unit = subject as Unit;
		unit.parentController = this;

		// init threat for other phases
		unit.UpdateThreatRange();
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
					
					StartCoroutine( SubjectsTakeAction() );
				}
				break;
				
			case Enum.PhaseActionState.acting:
				// do nothing until finished acting
				if (EndPhaseNow()) phaseActionState = Enum.PhaseActionState.complete;
				break;
				
			case Enum.PhaseActionState.complete:
				phaseActionState = Enum.PhaseActionState.postPhaseDelay;
				EndPhase(postPhaseDelay);
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
		List<MovingObject> activeRegistryClone = new List<MovingObject>(activeRegistry);

		for (int i = 0; i < activeRegistryClone.Count; i++) {
			Unit subject = (Unit)activeRegistryClone[i];
			var grid = GameManager.inst.tacticsManager.GetActiveGrid();
			
			// determine some order in which the enemies act
			// target selection
			// if outside of range, just move towards the closest adversary
			// TODO: if multiple adversaries in range, pick the adversary to which you can do the most damage
			subject.UpdateThreatRange();

			var targetPosition = GetTargetInAttackRange(subject, playerController.activeRegistry);
			if (targetPosition == -1*Vector3Int.one) {
				targetPosition = GetClosestTarget(subject, playerController.activeRegistry);
			}

			// now that we have the target location, find all the "attackable" squares surrounding it
			// then, select the optimal square to move into to perform your attack
			var optimalPosition = GetOptimalToAttack(subject, targetPosition);

			if (subject.OptionActive("Move") && optimalPosition != subject.gridPosition) {
				subject.SetOption("Move", false);

				// now, find a full path to the location
				// even if we can't reach it, just go as far as you can
				// CAVEAT: we can't just clip it
				// if we do, we can have enemies standing in the same place.
				// instead, we have to do the laborious thing, and REpath-find to the new clipped position
				var pathToTarget = MovingObjectPath.GetPathTo(subject.gridPosition, optimalPosition, subject.obstacles);
				var clippedPath  = MovingObjectPath.Clip(pathToTarget, subject.moveRange);

				// if the clipped path already has someone there... radiate again to find another place to stand nearby
				var finalPosition = NextVacantPos(subject, clippedPath.end);
				var finalPath = MovingObjectPath.GetPathTo(subject.gridPosition, finalPosition, subject.obstacles);
				//
				subject.TraverseTo(finalPath.end, fieldPath: finalPath);

				// spin until this subject is entirely done moving
				while (subject.IsMoving()) { yield return null; }
			}

			if (subject.OptionActive("Attack") && subject.attackRange.ValidAttack(subject, targetPosition)) {
				subject.SetOption("Attack", false);

				var unitAt = (Unit)grid.OccupantAt(targetPosition);
				var engagement = new Engagement(subject, unitAt);

				// wait until the engagement has ended
				StartCoroutine(engagement.ResolveResults());
				while (!engagement.resolved) { yield return null; }

				// wait until results have killed units, if necessary
				StartCoroutine(engagement.results.ResolveCasualties());
				while (!engagement.results.resolved) { yield return null; }
			}

			// finally:
			// this will discolor the unit and set its options to false, after movement is complete
			// BUT, don't let the other units move until this subject has finished
			subject.OnEndTurn();
			yield return new WaitForSeconds(0.5f);
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
		bool SubjectCanStand(Vector3Int v) {
			return grid.IsInBounds(v) && (grid.VacantAt(v) || v == subject.gridPosition);
		}
		float DistToTarget(Vector3Int v) { return targetPosition.ManhattanDistance(v); }
		float DistToSubject(Vector3Int v) { return subject.gridPosition.ManhattanDistance(v); }
		// util

		// max allowable attack positions (max range/reach)
		// NOTE: need to Radiate again if the sequence is empty.
		var targetable = targetPosition.GridRadiate(GameManager.inst.GetActiveGrid(), subject._RANGE).Where(it => SubjectCanStand(it));
		float maxDistWithin = targetable.Max(it => DistToTarget(it));
		var atMaxDist = targetable.Where(it => DistToTarget(it) == maxDistWithin);

		// the closest of those to the acting subject
		float minDistSubject = atMaxDist.Min(it => DistToSubject(it));
		var optimalPosition = atMaxDist.First(it => DistToSubject(it) == minDistSubject);
		return optimalPosition;
	}

	public Vector3Int NextVacantPos(Unit subject, Vector3Int origPos) {
		var grid = GameManager.inst.GetActiveGrid();
		if (!grid.VacantAt(origPos)) {
			foreach (var v in origPos.GridRadiate(grid, subject.MOVE)) {
				if (subject.moveRange.field.ContainsKey(v) && grid.VacantAt(v)) {
					return v;
				}
			}
		}
		return origPos;
	}


	public override HashSet<Vector3Int> GetObstacles() {		
		return GameManager.inst.GetActiveGrid().CurrentOccupantPositionsExcepting<EnemyUnit>();
	}
}