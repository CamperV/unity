using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Extensions;

public class EnemyUnitController : UnitController, IPhaseable
{
	// IPhaseable
	[HideInInspector] public float phaseDelayTime { get => 0f; } // in units of WaitForSeconds();
	[HideInInspector] public Enum.PhaseActionState phaseActionState { get; set; }

	private bool subjectsActingTrigger;
	
	void Awake() {
		subjectsActingTrigger = false;
	}

	void Start() {
		RegisterTo(Battle.active.turnManager);
	}

	// IPhaseable definitions
	public void RegisterTo(TurnManager manager) {
		manager.enemyPhase.StartEvent += TriggerPhase;
		manager.enemyPhase.EndEvent   += EndPhase;
	}
	
	public void TriggerPhase() {
		phaseActionState = Enum.PhaseActionState.waitingForInput;
		activeRegistry.ForEach(u => ((Unit)u).OnStartTurn());
		subjectsActingTrigger = true;
	}

	public void EndPhase() {
		// then reset your phase, and mark as complete
		StartCoroutine(Utils.DelayedExecute(postPhaseDelayTime, () => {
			phaseActionState = Enum.PhaseActionState.postPhase;
		}));
		activeRegistry.ForEach(u => (u as Unit).RefreshColor());
	}
	// IPhaseable definitions

	public override void Register(MovingGridObject subject) {
		base.Register(subject);
		//
		Unit unit = subject as Unit;
		unit.parentController = this;
	}

	public override List<MovingGridObject> GetOpposing() {
		return Battle.active.GetControllerFromTag("EnemyArmy").activeRegistry;
	}

	public override HashSet<Vector3Int> GetObstacles() {		
		return Battle.active.grid.CurrentOccupantPositionsExcepting<EnemyUnit>();
	}
	
	void Update() {
		// if (!MyPhaseActive()) return;
		if (phaseActionState == Enum.PhaseActionState.inactive) return;
		
		switch(phaseActionState) {
			case Enum.PhaseActionState.inactive:
				break;
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
				EndPhase();
				Battle.active.turnManager.enemyPhase.TriggerEnd();
				break;
				
			// delay for phaseDelayTime, until you go into postPhase
			case Enum.PhaseActionState.postPhaseDelay:
			case Enum.PhaseActionState.postPhase:
				break;
		}
	}

	public IEnumerator SubjectsTakeAction() {
		List<MovingGridObject> activeRegistryClone = new List<MovingGridObject>(activeRegistry);

		for (int i = 0; i < activeRegistryClone.Count; i++) {
			Unit subject = (Unit)activeRegistryClone[i];
			
			// determine some order in which the enemies act
			// target selection
			// if outside of range, just move towards the closest adversary
			// TODO: if multiple adversaries in range, pick the adversary to which you can do the most damage
			subject.UpdateThreatRange();

			List<PlayerUnit> targets = Battle.active.RegisteredUnits.OfType<PlayerUnit>().ToList();

			Vector3Int targetPosition = GetTargetInAttackRange(subject, targets);
			if (targetPosition == Constants.unselectableVector3Int) {
				targetPosition = GetClosestTarget(subject, targets);
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
				Debug.Log($"{subject}@{subject.gridPosition} found {optimalPosition} to attack {targetPosition}");
				Path pathToTarget = new UnitPathfinder(subject.obstacles).BFS<Path>(subject.gridPosition, optimalPosition);
				pathToTarget.Clip(subject.moveRange);

				// if the clipped path already has someone there... radiate again to find another place to stand nearby
				if (!Battle.active.grid.VacantAt(pathToTarget.end)) {
					Vector3Int finalPosition = NextVacantPos(subject, pathToTarget.end);
					pathToTarget = new UnitPathfinder(subject.obstacles).BFS<Path>(subject.gridPosition, finalPosition);
				}
				//
				subject.TraverseTo(pathToTarget.end, pathToTarget);

				// spin until this subject is entirely done moving
				while (subject.spriteAnimator.isMoving) { yield return null; }
			}

			if (subject.OptionActive("Attack") && subject.attackRange.ValidAttack(subject, targetPosition)) {
				subject.SetOption("Attack", false);

				var unitAt = (Unit)Battle.active.grid.OccupantAt(targetPosition);
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

	private Vector3Int GetTargetInAttackRange(Unit subject, List<PlayerUnit> targets) {
		var attackTargets = targets.FindAll(it => subject.attackRange.field.ContainsKey(it.gridPosition));
		return (attackTargets.Any()) ? GetClosestTarget(subject, attackTargets) : Constants.unselectableVector3Int;
	}

	private Vector3Int GetClosestTarget(Unit subject, List<PlayerUnit> targets) {
		var minDist = targets.Min(it => it.gridPosition.ManhattanDistance(subject.gridPosition));
		return targets.First(it => it.gridPosition.ManhattanDistance(subject.gridPosition) == minDist).gridPosition;
	}

	private Vector3Int GetOptimalToAttack(Unit subject, Vector3Int targetPosition) {
		bool SubjectCanStand(Vector3Int v) {
			return Battle.active.grid.IsInBounds(v) && (Battle.active.grid.VacantAt(v) || v == subject.gridPosition);
		}
		float DistToTarget(Vector3Int v) { return targetPosition.ManhattanDistance(v); }
		float DistToSubject(Vector3Int v) { return subject.gridPosition.ManhattanDistance(v); }
		// util

		// max allowable attack positions (max range/reach)
		// NOTE: need to Radiate again if the sequence is empty.
		var targetable = targetPosition.GridRadiate(Battle.active.grid, subject._RANGE).Where(it => SubjectCanStand(it));
		float maxDistWithin = targetable.Max(it => DistToTarget(it));
		var atMaxDist = targetable.Where(it => DistToTarget(it) == maxDistWithin);

		// the closest of those to the acting subject
		float minDistSubject = atMaxDist.Min(it => DistToSubject(it));
		var optimalPosition = atMaxDist.First(it => DistToSubject(it) == minDistSubject);
		return optimalPosition;
	}

	public Vector3Int NextVacantPos(Unit subject, Vector3Int origPos) {
		foreach (var v in origPos.GridRadiate(Battle.active.grid, subject.MOVE)) {
			if (subject.moveRange.field.ContainsKey(v) && Battle.active.grid.VacantAt(v)) {
				return v;
			}
		}
		return origPos;
	}
}