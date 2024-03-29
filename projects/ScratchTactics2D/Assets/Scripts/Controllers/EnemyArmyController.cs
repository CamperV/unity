﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class EnemyArmyController : Controller, IPhaseable
{
	// IPhaseable
	[HideInInspector] public float phaseDelayTime { get => 0f; } // in units of WaitForSeconds();
	[HideInInspector] public Enum.PhaseActionState phaseActionState { get; set; }

	// flags 
	private bool subjectsActingTrigger;
	private bool keepPhaseAlive;
	private bool crtActing;

	public Vector3Int lastKnownPlayerPos;
	public Vector3Int playerPosLastTurn;
	public FlowField flowFieldToPlayer;
	
	public HashSet<Vector3Int> traversablePositions = new HashSet<Vector3Int>();
	public HashSet<Vector3Int> untraversablePositions {
		get {
			HashSet<Vector3Int> retVal = new HashSet<Vector3Int>(GameManager.inst.overworld.Positions);
			retVal.ExceptWith(traversablePositions);
			return retVal;
		}
	}

	public HashSet<Vector3Int> currentEnemyPositions {
		get => GameManager.inst.overworld.CurrentOccupantPositions<EnemyArmy>();
	}

	public bool enemiesFollowing {
		get => activeRegistry.Where(it => (it as EnemyArmy).state == Enum.EnemyArmyState.followField).Any();
	}

	void Awake() {
		subjectsActingTrigger = false;
		keepPhaseAlive = false;
		crtActing = false;
		flowFieldToPlayer = new FlowField();
    }

	void Start() {
		RegisterTo(GameManager.inst.overworld.turnManager);
	}

	// IPhaseable definitions
	public void RegisterTo(TurnManager manager) {
		manager.enemyPhase.StartEvent += TriggerPhase;
		manager.enemyPhase.EndEvent   += EndPhase;
		Debug.Log($"Registered {this} to {GameManager.inst.overworld.turnManager.enemyPhase}");
	}
	
	// IPhaseable definitions
	public void TriggerPhase() {
		Debug.Log($"Enemy army triggerPhase");
		phaseActionState = Enum.PhaseActionState.waitingForInput;
		subjectsActingTrigger = true;
	}

	public void EndPhase() {
		// then reset your phase, and mark as complete
		StartCoroutine(Utils.DelayedExecute(phaseDelayTime, () => {
			phaseActionState = Enum.PhaseActionState.postPhase;
		})); 
	}
	// IPhaseable definitions

	public override void Register(MovingGridObject subject) {
		base.Register(subject);
		(subject as EnemyArmy).ID = registry.Count;
	}

    void Update() {
        // if (!MyPhaseActive()) return;
		if (phaseActionState == Enum.PhaseActionState.inactive) return;
		//
		switch(phaseActionState) {
			case Enum.PhaseActionState.inactive:
				break;
			case Enum.PhaseActionState.waitingForInput:
				// start action coroutine if not currently running
				// reset trigger immediately
				if (subjectsActingTrigger) {
					subjectsActingTrigger = false;
					phaseActionState = Enum.PhaseActionState.acting;
					
					// update player position
					playerPosLastTurn = lastKnownPlayerPos;
					lastKnownPlayerPos = GlobalPlayerState.army.gridPosition;
					
					// update the field once for all
					// and then, we'll update them again every time a subject moves
					if (HasPlayerMoved() && Reachable(lastKnownPlayerPos)) {
						UpdateFlowField();
					}
					StartCoroutine( SubjectsTakeAction() );
				}
				break;
				
			case Enum.PhaseActionState.acting:
				// only end the phase if there are no available options to activated enemies
				if (!crtActing) {
					if (keepPhaseAlive) {
						keepPhaseAlive = false;

						// start anew
						TriggerPhase();
					} else {
						phaseActionState = Enum.PhaseActionState.complete;
					}
				}
				break;
				
			case Enum.PhaseActionState.complete:
				phaseActionState = Enum.PhaseActionState.postPhaseDelay;
				GameManager.inst.overworld.turnManager.enemyPhase.TriggerEnd();
				break;
				
			// delay for phaseDelayTime, until you go into postPhase
			case Enum.PhaseActionState.postPhaseDelay:
			case Enum.PhaseActionState.postPhase:
				phaseActionState = Enum.PhaseActionState.inactive;
				break;
		}
    }
	
	public IEnumerator SubjectsTakeAction() {
		// use this bool to determine whether or not to start the phase over again
		// this allows all enemies to spend out their ticks properly
		keepPhaseAlive = false;
		crtActing = true;

		List<MovingGridObject> orderedRegistry = activeRegistry.OrderBy(it => (it as EnemyArmy).CalculateInitiative()).ToList();
		for (int i = 0; i < orderedRegistry.Count; i++) {
			EnemyArmy subject = orderedRegistry[i] as EnemyArmy;

			switch(subject.state) {
				case Enum.EnemyArmyState.idle:
					// alert! animation
					if (subject.InDetectionRange(flowFieldToPlayer)) {
						subject.state = Enum.EnemyArmyState.followField;
						subject.OnAlert();

						// also tell the PlayerArmyController to clear its queue
						// this gives the player a chance to jump out of a pre-determined path (mouse-move)
						GlobalPlayerState.controller.ClearActionQueue();
					} else {
						subject.TakeIdleAction();
					}
					break;
				
				// this state requires ticks to function
				// tickpool is managed in the subject class, but we can tell it to keep moving here
				case Enum.EnemyArmyState.followField:
					// if we can attack, do that with a higher priority
					if (subject.CanAttackPlayer()) {	// checks ticks
						Debug.Log($"{subject} can attack player");

						// ...and spends ticks
						if (Battle.active) {
							subject.OnAlert();
							subject.JoinBattle();
						} else {
							subject.InitiateBattle();
						}
						subject.state = Enum.EnemyArmyState.inBattle;

						// since initing/joining a battle takes all ticks:
						// (does nothing of course), just here for clarity
						keepPhaseAlive |= false;

					// otherwise, move via FlowField
					// checks and spends ticks
					} else {
						FlowField subjectField = IndividualFlowField(subject);
						keepPhaseAlive |= subject.FollowField(subjectField, GlobalPlayerState.army);
					}
					while (subject.spriteAnimator.isMoving) yield return null;
					break;

				case Enum.EnemyArmyState.inBattle:
					break;
					
				// end case
			}
		}

		// suitable pause to see that the units are moving again
		if (keepPhaseAlive) yield return new WaitForSeconds(0.10f);
		crtActing = false;
	}
		
	public bool HasPlayerMoved() {
		return playerPosLastTurn != GlobalPlayerState.army.gridPosition;
	}
	
	public void SetTraversableTiles() {
		// traversable is a hash set of all tiles that we can draw a flowfield on
		// this method will check tile types to see if they're traversable
		// it will also grow the tiles from subject placements, such that any
		// "island" that is unspawnable around, but traversable inside, will be eliminated
		traversablePositions.Clear();
		
		// for each subject, grow a region to create traversable
		foreach (var subject in activeRegistry) {
			EnemyArmy enemy = (EnemyArmy)subject;
			Queue<Vector3Int> queue = new Queue<Vector3Int>();

			// initial setup
			Vector3Int currentPos;		
			queue.Enqueue(enemy.gridPosition);

			while (queue.Count != 0) {
				currentPos = queue.Dequeue();
				
				// will return in-bounds neighbors
				// if: we've never seen them before
				// if: they are not unspawnable
				foreach (Vector3Int adjacent in GameManager.inst.overworld.GetNeighbors(currentPos)) {
					if (traversablePositions.Contains(adjacent))
						continue;

					traversablePositions.Add(adjacent);
					queue.Enqueue(adjacent);
				}
			}
		}
	}
	
	public void InitFlowField(Vector3Int initOrigin) {
		flowFieldToPlayer = new ArmyPathfinder(untraversablePositions, EnemyArmy.globalMoveThreshold).FlowField<FlowField>(initOrigin);
	}
	
	private void UpdateFlowField() {
		FlowField prevFlowFieldToPlayer = flowFieldToPlayer;
		
		flowFieldToPlayer = new ArmyPathfinder(untraversablePositions, EnemyArmy.globalMoveThreshold).FlowField<FlowField>(lastKnownPlayerPos);
		flowFieldToPlayer.Absorb(prevFlowFieldToPlayer);
	}

	private FlowField IndividualFlowField(EnemyArmy subject) {
		// each enemy needs to smartly avoid each other as to not pile up
		// Do not make a new FlowField, but rather patch the current FlowField to add cost to spots currently occupied by friendlies
		FlowField patchedFlowField = flowFieldToPlayer;

		foreach (Vector3Int enemyPos in currentEnemyPositions) {
			if (enemyPos == subject.gridPosition) continue;
			if (patchedFlowField.field.ContainsKey(enemyPos)) {
				patchedFlowField.field[enemyPos] += EnemyArmy.globalMoveThreshold;
			} else {
				patchedFlowField.field[enemyPos] = EnemyArmy.globalMoveThreshold;
			}
		}
		return patchedFlowField;
	}
	
	public bool Reachable(Vector3Int pos) {
		List<Vector3Int> neighbors = new List<Vector3Int>() {
			pos + Vector3Int.up,	// N
			pos + Vector3Int.right,	// E
			pos + Vector3Int.down,	// S
			pos + Vector3Int.left	// W
		};
		
		// break out early and confirm true if any neighbors to this position are traversable
		foreach(Vector3Int neighbor in neighbors) {
			if (traversablePositions.Contains(neighbor)) {
				return true;
			}
		}
		
		return false;
	}

	public void AddTicksAll(int ticks) {
		// (implicitly re-box cast)
		// only receive ticks if you are currently activated, and moving
		// if you're inBattle or idle, do not receive ticks
		foreach (EnemyArmy en in activeRegistry) {
			if (en.state == Enum.EnemyArmyState.followField) {
				en.AddTicks(ticks);
			}
		}
	}
}
