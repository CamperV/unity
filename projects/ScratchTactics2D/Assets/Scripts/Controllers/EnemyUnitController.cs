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
	
	void Awake() {
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

		// find initial target when there are no adversaries in one's AttackRange
		Vector3 _oppositionCentroid = Vector3.zero;
		foreach (var adv in playerController.activeRegistry) {
			_oppositionCentroid += adv.gridPosition;
		}
		_oppositionCentroid /= (playerController.activeRegistry.Count);
		Vector3Int oppositionCentroid = new Vector3Int(Mathf.RoundToInt(_oppositionCentroid.x),
													   Mathf.RoundToInt(_oppositionCentroid.y),
													   Mathf.RoundToInt(_oppositionCentroid.z));
		oppositionCentroid = playerController.activeRegistry[0].gridPosition;

		for (int i = 0; i < activeRegistry.Count; i++) {
			Unit subject = (Unit)activeRegistry[i];
			
			// determine some order in which the enemies act
			// target selection
			// if outside of range, just move towards the closest adversary
			// if multiple adversaries in range, pick the adversary to which you can do the most damage
			subject.UpdateThreatRange();

			var targetPosition = GetTargetInAttackRange(subject, playerController.activeRegistry);
			if (targetPosition == subject.gridPosition) targetPosition = oppositionCentroid;

			// now that we have the target location, find all the "attackable" squares surrounding it
			// then, select the optimal square to move into to perform your attack
			Vector3Int optimalPosition = -1*Vector3Int.one;
			float currMin = float.MaxValue;
			//
			//foreach (var attackablePos in targetPosition.Radiate(subject.attackReach)) {
			foreach (var attackablePos in targetPosition.Radiate(subject.movementRange)) {
				var currDis = (subject.gridPosition - attackablePos).magnitude;

				// also, enforce some global rules. No unit can stand in the same
				// place as another, not only if they are considered obstacles
				// for unoccupied: only check vacancy if you're not the one there ;)
				var grid = GameManager.inst.tacticsManager.GetActiveGrid();
				var inBounds   = grid.IsInBounds(attackablePos);
				var unoccupied = (currDis == 0f) ? true : grid.VacantAt(attackablePos);

				if (inBounds && unoccupied && currDis < currMin) {
					optimalPosition = attackablePos;
					currMin = currDis;
				}
			}
			Debug.Log($"{this} found opt {optimalPosition} to attack {targetPosition}, reach {subject.attackReach}");

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

			if (subject.OptionActive("Attack")) {
				//
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
		// for right now, let's just attack the closest
		Vector3Int finalTarget = subject.gridPosition;

		foreach (var target in targets) {
			if (subject.attackRange.field.ContainsKey(target.gridPosition)) {
				if (finalTarget == subject.gridPosition) {
					finalTarget = target.gridPosition;
					break;
				}
				var targetDistance = (subject.gridPosition - target.gridPosition).magnitude;
				if (targetDistance < (subject.gridPosition - finalTarget).magnitude) {
					finalTarget = target.gridPosition;
				}
			}
		}
		return finalTarget;
	}
}