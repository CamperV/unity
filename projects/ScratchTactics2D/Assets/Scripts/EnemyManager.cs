using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class EnemyManager : PhasedObject, IPhasedObject
{
	private List<Enemy> enemyList;
	
	// flags 
	private bool subjectsActingTrigger;

	public Vector3Int lastKnownPlayerPos;
	public Vector3Int playerPosLastTurn;
	public FlowField flowFieldToPlayer;
	
	public HashSet<Vector3Int> traversable = new HashSet<Vector3Int>();
	public HashSet<Vector3Int> currentEnemyPositions = new HashSet<Vector3Int>();

	void Awake() {
		myPhase = Enum.Phase.enemy;
		subjectsActingTrigger = false;
		
		enemyList = new List<Enemy>();
		flowFieldToPlayer = new FlowField();
    }

    void Update() {
        if (!MyPhaseActive()) return;
		//
		switch(phaseActionState) {
			case Enum.PhaseActionState.waitingForInput:
				TakePhaseAction();
				break;
			case Enum.PhaseActionState.acting:
				// do nothing until finished acting
				break;
			case Enum.PhaseActionState.complete:
				phaseActionState = Enum.PhaseActionState.postPhaseDelay;
				EndPhase();
				break;
			case Enum.PhaseActionState.postPhaseDelay:
				// delay for phaseDelayTime, until you go into postPhase
			case Enum.PhaseActionState.postPhase:
				break;
		}
    }
	
	// overrides base
	public void TriggerPhase() {
		base.TriggerPhase();
		subjectsActingTrigger = true;
	}
	
	public override void TakePhaseAction() {	
		// start action coroutine if not currently running
		// reset trigger immediately
		if (subjectsActingTrigger) {
			subjectsActingTrigger = false;
			phaseActionState = Enum.PhaseActionState.acting;
			
			// update player position
			playerPosLastTurn = lastKnownPlayerPos;
			lastKnownPlayerPos = GameManager.inst.player.gridPosition;
			UpdateFlowField();

			StartCoroutine(SubjectTakePhaseActions());
		}
	}
	
	public IEnumerator SubjectTakePhaseActions() {
		for (int i = 0; i < enemyList.Count; i++) {		
			switch(enemyList[i].state) {
				case Enum.EnemyState.idle:
					if (enemyList[i].InDetectionRange(flowFieldToPlayer)) {
						enemyList[i].state = Enum.EnemyState.followField;
						// alert! animation
						enemyList[i].Alert();
					} else {
						enemyList[i].TakeIdleAction();
					}
					break;
				case Enum.EnemyState.followField:
					enemyList[i].FollowField(flowFieldToPlayer);
					break;
				case Enum.EnemyState.pathing:
					enemyList[i].MoveTowardsPosition(lastKnownPlayerPos);
					break;
			}
			
			// don't delay if you're the last
			if (i == enemyList.Count-1 || enemyList[i].state == Enum.EnemyState.idle) {
				yield return null;
			} else {
				yield return new WaitForSeconds(phaseDelayTime);
			}
		}
		
		phaseActionState = Enum.PhaseActionState.complete;
	}
		
	public bool HasPlayerMoved() {
		return playerPosLastTurn != GameManager.inst.player.gridPosition;
	}
	
	public void SetTraversableTiles() {
		// traversable is a hash set of all tiles that we can draw a flowfield on
		// this method will check tile types to see if they're traversable
		// it will also grow the tiles from enemy placements, such that any
		// "island" that is untraversable around, but traversable inside, will be eliminated
		traversable.Clear();
		
		// for each enemy, grow a region to create traversable
		foreach (Enemy enemy in enemyList) {
			Queue<Vector3Int> queue = new Queue<Vector3Int>();

			// initial setup
			Vector3Int currentPos;		
			queue.Enqueue(enemy.gridPosition);

			while (queue.Count != 0) {
				currentPos = queue.Dequeue();
				
				// will return in-bounds neighbors
				// if: we've never seen them before
				// if: they are not untraversable
				foreach (Vector3Int adjacent in GameManager.inst.worldGrid.GetNeighbors(currentPos)) {
					if (traversable.Contains(adjacent))
						continue;
					if (Enemy.untraversable.Contains(GameManager.inst.worldGrid.GetWorldTileAt(adjacent).GetType()))
						continue;
					
					traversable.Add(adjacent);
					queue.Enqueue(adjacent);
				}
			}
		}
	}
	
	public void InitFlowField(Vector3Int initOrigin) {
		flowFieldToPlayer = FlowField.FlowFieldFrom(initOrigin, traversable);
		flowFieldToPlayer.Display();
	}
	
	public void UpdateFlowField() {
		if (HasPlayerMoved() && Reachable(lastKnownPlayerPos)) {
			FlowField prevFlowFieldToPlayer = flowFieldToPlayer;
			
			flowFieldToPlayer = FlowField.FlowFieldFrom(lastKnownPlayerPos, traversable);
			flowFieldToPlayer.Absorb(prevFlowFieldToPlayer);
			flowFieldToPlayer.Display();
		}
	}
	
	public void UpdateCurrentEnemyPositions() {
		currentEnemyPositions = GameManager.inst.worldGrid.CurrentOccupantPositions<Enemy>();
	}
		
	public void AddSubject(Enemy enemy) {
		enemyList.Add(enemy);
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
}
