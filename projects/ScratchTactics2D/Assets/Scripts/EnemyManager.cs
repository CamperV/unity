using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour, IPhasedObject
{
	private List<Enemy> enemyList;
	private HashSet<Vector3Int> _pathTiles;
	
	private bool subjectsActingTrigger;
	private bool allSubjectsPhaseActionTaken;
	
	[HideInInspector] public bool phaseActionTaken { get; private set; }

	public Vector3Int lastKnownPlayerPos;
	public Vector3Int playerPosLastTurn;

	
	void Awake() {
		phaseActionTaken = false;
		subjectsActingTrigger = false;
		allSubjectsPhaseActionTaken = false;
		
		enemyList = new List<Enemy>();
		_pathTiles = new HashSet<Vector3Int>();
    }

    void Update() {
        if (!MyPhase()) return;
		phaseActionTaken = TakePhaseAction();
    }
	
	public bool MyPhase() {
		return GameManager.inst.phaseManager.currentPhase == PhaseManager.Phase.enemy && phaseActionTaken == false;
	}
	
	public void TriggerPhase() {
		phaseActionTaken = false;
		subjectsActingTrigger = true;
	}
	
	public bool TakePhaseAction() {
		// start action coroutine if not currently running
		// reset trigger immediately
		if (subjectsActingTrigger) {
			subjectsActingTrigger = false;
			
			// update player position
			playerPosLastTurn = lastKnownPlayerPos;
			lastKnownPlayerPos = GameManager.inst.player.gridPosition;

			allSubjectsPhaseActionTaken = false;
			StartCoroutine(SubjectTakePhaseActions());
		}
		
		// find a way to make this persist
		GameManager.inst.worldGrid.ResetHighlightTiles(_pathTiles);
		_pathTiles.Clear();
		
		foreach (Enemy enemy in enemyList) {
			foreach (Vector3Int tile in enemy.pathToPlayer.path.Keys) {
				_pathTiles.Add(tile);
			}
		}
		GameManager.inst.worldGrid.HighlightTiles(_pathTiles, Color.red);
		
		return allSubjectsPhaseActionTaken;
	}
	
	public void AddSubject(Enemy enemy) {
		enemyList.Add(enemy);
	}
	
	public IEnumerator SubjectTakePhaseActions() {
		bool pAT = true;
		
		for (int i = 0; i < enemyList.Count; i++) {
			if (i == enemyList.Count) {
				yield return null;
			} else {
				yield return new WaitForSeconds(enemyList[i].moveDelayTime);
			}
			
			pAT &= enemyList[i].TakePhaseAction();
		}
		
		// pAT isn't necessary, but may be in the future
		if (pAT) {
			allSubjectsPhaseActionTaken = true;
		}
	}
	
	public bool HasPlayerMoved() {
		return playerPosLastTurn != GameManager.inst.player.gridPosition;
	}
	
	public List<Vector3Int> GetMovementOptions(Vector3Int fromPosition) {
		// since we call TakePhaseAction serially...
		// we don't need to know if an Enemy WILL move into a spot.
		// if they had higher priority, they will have already moved into it	
		// also, the conversion to HashSet, and the conversion back, is not worth it to remove from a list of 4 spaces max
		List<Vector3Int> moveOptions = new List<Vector3Int>();
		
		foreach (Vector3Int pos in GameManager.inst.worldGrid.GetNeighbors(fromPosition)) {
			if (!GameManager.inst.worldGrid.IsInBounds(pos)) {
				continue;
			}
			var occupant = GameManager.inst.worldGrid.OccupantAt(pos);
			
			// either check the tag or type of occupant
			// if occupant is null, short-circuit and add moveOption
			// if it is occupied, but is a Player, still works
			if (occupant != null && occupant.GetType() != typeof(Player)) {
				continue;
			}
			moveOptions.Add(pos);
		}
		return moveOptions;	
		
	}
}
