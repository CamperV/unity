using System.Collections;
using System.Collections.Generic;
using System;
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
	
	public HashSet<Vector3Int> traversable = new HashSet<Vector3Int>();
	public HashSet<Vector3Int> currentEnemyPositions = new HashSet<Vector3Int>();

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
					UpdateFlowField();

					StartCoroutine(SubjectsTakeAction());
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

		for (int i = 0; i < activeRegistry.Count; i++) {
			OverworldEnemyBase subject = (OverworldEnemyBase)activeRegistry[i];
			
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
					keepPhaseAlive |= subject.FollowField(flowFieldToPlayer, GameManager.inst.player);
					break;
			}
			
			// don't delay if you're the last/idle
			if (i == activeRegistry.Count-1 || subject.state == Enum.EnemyState.idle) {
				yield return null;
			} else {
				yield return new WaitForSeconds(phaseDelayTime);
			}
		}

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
		traversable.Clear();
		
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
					if (traversable.Contains(adjacent))
						continue;

					traversable.Add(adjacent);
					queue.Enqueue(adjacent);
				}
			}
		}
	}
	
	public void InitFlowField(Vector3Int initOrigin) {
		flowFieldToPlayer = FlowField.FlowFieldFrom(initOrigin, traversable);
		//flowFieldToPlayer.Display();
	}
	
	public void UpdateFlowField() {
		if (HasPlayerMoved() && Reachable(lastKnownPlayerPos)) {
			FlowField prevFlowFieldToPlayer = flowFieldToPlayer;
			
			flowFieldToPlayer = FlowField.FlowFieldFrom(lastKnownPlayerPos, traversable);
			flowFieldToPlayer.Absorb(prevFlowFieldToPlayer);
			//flowFieldToPlayer.Display();
		}
	}
	
	public void UpdateCurrentEnemyPositions() {
		currentEnemyPositions = GameManager.inst.worldGrid.CurrentOccupantPositions<OverworldEnemyBase>();
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
			if (traversable.Contains(neighbor)) {
				return true;
			}
		}
		
		return false;
	}

	public void AddTicksAll(int ticks) {
		// (implicitly re-box cast)
		foreach (OverworldEnemyBase en in activeRegistry) {
			if (en.state != Enum.EnemyState.idle) {
				en.AddTicks(ticks);
			}
		}
	}
}
