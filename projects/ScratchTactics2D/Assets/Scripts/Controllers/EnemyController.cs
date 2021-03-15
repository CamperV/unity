using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class EnemyController : Controller
{
	// flags 
	private bool subjectsActingTrigger;
	private bool keepPhaseAlive;
	private bool crtActing;

	public Vector3Int lastKnownPlayerPos;
	public Vector3Int playerPosLastTurn;
	public FlowField flowFieldToPlayer;
	
	public HashSet<Vector3Int> traversablePositions = new HashSet<Vector3Int>();
	public HashSet<Vector3Int> currentEnemyPositions {
		get => GameManager.inst.worldGrid.CurrentOccupantPositions<OverworldEnemyBase>();
	}

	protected override void Awake() {
		base.Awake();
		//
		myPhase = Enum.Phase.enemy;
		subjectsActingTrigger = false;
		keepPhaseAlive = false;
		crtActing = false;
		flowFieldToPlayer = new FlowField();
    }

	public override bool MyPhaseActive() {
		return GameManager.inst.phaseManager.currentPhase == myPhase &&
			   GameManager.inst.gameState == Enum.GameState.overworld;
	}

    void Update() {
        if (!MyPhaseActive()) return;
		//
		switch(phaseActionState) {
			case Enum.PhaseActionState.waitingForInput:
				// start action coroutine if not currently running
				// reset trigger immediately
				if (subjectsActingTrigger) {
					subjectsActingTrigger = false;
					phaseActionState = Enum.PhaseActionState.acting;
					
					// update player position
					playerPosLastTurn = lastKnownPlayerPos;
					lastKnownPlayerPos = GameManager.inst.player.gridPosition;
					
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
				EndPhase();
				break;
				
			// delay for phaseDelayTime, until you go into postPhase
			case Enum.PhaseActionState.postPhaseDelay:
			case Enum.PhaseActionState.postPhase:
				break;
		}
    }
	
	// overrides base
	public override void TriggerPhase() {
		base.TriggerPhase();
		subjectsActingTrigger = true;
	}
	
	public IEnumerator SubjectsTakeAction() {
		// use this bool to determine whether or not to start the phase over again
		// this allows all enemies to spend out their ticks properly
		keepPhaseAlive = false;
		crtActing = true;

		List<MovingObject> orderedRegistry = activeRegistry.OrderBy(it => (it as OverworldEnemyBase).CalculateInitiative()).ToList();
		for (int i = 0; i < orderedRegistry.Count; i++) {
			OverworldEnemyBase subject = orderedRegistry[i] as OverworldEnemyBase;

			switch(subject.state) {
				case Enum.EnemyState.idle:
					// alert! animation
					if (subject.InDetectionRange(flowFieldToPlayer)) {
						subject.state = Enum.EnemyState.followField;
						subject.Alert();
					} else {
						subject.TakeIdleAction();
					}
					break;
				
				// this state requires ticks to function
				// tickpool is managed in the subject class, but we can tell it to keep moving here
				case Enum.EnemyState.followField:
					// if we can attack, do that with a higher priority
					if (subject.CanAttackPlayer()) {	// checks ticks
						// ...and spends ticks
						if (GameManager.inst.tacticsManager.activeBattle) {
							subject.Alert();
							subject.JoinBattle();
						} else {
							subject.InitiateBattle();
						}

						// since initing/joining a battle takes all ticks:
						// (does nothing of course), just here for clarity
						keepPhaseAlive |= false;
						break;

					// otherwise, move via FlowField
					// checks and spends ticks
					} else {
						FlowField subjectField = IndividualFlowField(subject);
						keepPhaseAlive |= subject.FollowField(subjectField, GameManager.inst.player);
						break;
					}

				case Enum.EnemyState.inBattle:
					Debug.Log($"{subject} will not do anything other than fight for its life, as it is currently.");
					break;
					
				// end case
			}
			
			// don't delay if you're the last/idle
			if (i == activeRegistry.Count-1 || subject.state == Enum.EnemyState.idle) {
				yield return null;
			} else {
				yield return new WaitForSeconds(phaseDelayTime);
			}
		}

		// suitable pause to see that the units are moving again
		if (keepPhaseAlive) yield return new WaitForSeconds(phaseDelayTime*10);
		crtActing = false;
	}
		
	public bool HasPlayerMoved() {
		return playerPosLastTurn != GameManager.inst.player.gridPosition;
	}
	
	public void SetTraversableTiles() {
		// traversable is a hash set of all tiles that we can draw a flowfield on
		// this method will check tile types to see if they're traversable
		// it will also grow the tiles from subject placements, such that any
		// "island" that is unspawnable around, but traversable inside, will be eliminated
		traversablePositions.Clear();
		
		// for each subject, grow a region to create traversable
		foreach (var subject in activeRegistry) {
			OverworldEnemyBase enemy = (OverworldEnemyBase)subject;
			Queue<Vector3Int> queue = new Queue<Vector3Int>();

			// initial setup
			Vector3Int currentPos;		
			queue.Enqueue(enemy.gridPosition);

			while (queue.Count != 0) {
				currentPos = queue.Dequeue();
				
				// will return in-bounds neighbors
				// if: we've never seen them before
				// if: they are not unspawnable
				foreach (Vector3Int adjacent in GameManager.inst.worldGrid.GetNeighbors(currentPos)) {
					if (traversablePositions.Contains(adjacent))
						continue;

					traversablePositions.Add(adjacent);
					queue.Enqueue(adjacent);
				}
			}
		}
	}
	
	public void InitFlowField(Vector3Int initOrigin) {
		flowFieldToPlayer = FlowField.FlowFieldFrom(initOrigin, traversablePositions);
	}
	
	private void UpdateFlowField() {
		FlowField prevFlowFieldToPlayer = flowFieldToPlayer;
		
		flowFieldToPlayer = FlowField.FlowFieldFrom(lastKnownPlayerPos, traversablePositions);
		flowFieldToPlayer.Absorb(prevFlowFieldToPlayer);
	}

	private FlowField IndividualFlowField(OverworldEnemyBase subject) {
		// each enemy needs to smartly avoid each other as to not pile up
		// Do not make a new FlowField, but rather patch the current FlowField to add cost to spots currently occupied by friendlies
		FlowField patchedFlowField = flowFieldToPlayer;

		foreach (Vector3Int enemyPos in currentEnemyPositions) {
			if (enemyPos == subject.gridPosition) continue;
			patchedFlowField.field[enemyPos] += 100;
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
		foreach (OverworldEnemyBase en in activeRegistry) {
			if (en.state == Enum.EnemyState.followField) {
				en.AddTicks(ticks);
			}
		}
	}
}
