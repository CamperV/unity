using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, IPhasedObject
{
	private List<Enemy> enemyList;
	
	public bool phaseActionTaken { get; set; }
	public Vector3Int lastKnownPlayerPos;
	
	void Awake() {
		phaseActionTaken = false;
		enemyList = new List<Enemy>();
    }

    void Update() {
        if (!MyPhase()) return;
		
		phaseActionTaken = TakePhaseAction();
    }
	
	public bool MyPhase() {
		return GameManager.inst.currentPhase == GameManager.Phase.enemy && phaseActionTaken == false;
	}
	
	public bool TakePhaseAction() {
		// update player position
		lastKnownPlayerPos = GameManager.inst.player.gridPosition;
		
		bool pAT = true;
		foreach (Enemy enemy in enemyList) {
			pAT &= enemy.TakePhaseAction();
		}
		
		return pAT;
	}
	
	public void AddSubject(Enemy enemy) {
		enemyList.Add(enemy);
	}
	
	public bool HasPlayerMoved() {
		return lastKnownPlayerPos == GameManager.inst.player.gridPosition;
	}
	
	public List<Vector3Int> GetMovementOptions(Vector3Int fromPosition) {
		// since we call TakePhaseAction serially...
		// we don't need to know if an Enemy WILL move into a spot.
		// if they had higher priority, they will have already moved into it
		List<Vector3Int> currentEnemyPositions = new List<Vector3Int>();
		foreach (Enemy enemy in enemyList) {
			currentEnemyPositions.Add(enemy.gridPosition);
		}
		
		// also, the conversion to HashSet, and the conversion back, is not worth it to remove from a list of spaces max
		List<Vector3Int> moveOptions = new List<Vector3Int>();
		foreach (Vector3Int pos in GameManager.inst.worldGrid.GetNeighbors(fromPosition)) {
			if (!currentEnemyPositions.Contains(pos))
				moveOptions.Add(pos);
		}
		return moveOptions;	
		
	}
}
