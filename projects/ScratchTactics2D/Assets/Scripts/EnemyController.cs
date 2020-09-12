using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, IPhasedObject
{
	private List<Enemy> enemyList;
	private HashSet<Vector3Int> _pathTiles;
	
	public bool phaseActionTaken { get; set; }
	public Vector3Int lastKnownPlayerPos;
	public Vector3Int playerPosLastTurn;
	
	void Awake() {
		phaseActionTaken = false;
		//lastKnownPlayerPos = new Vector3Int(-1, -1, -1);
		
		enemyList = new List<Enemy>();
		_pathTiles = new HashSet<Vector3Int>();
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
		playerPosLastTurn = lastKnownPlayerPos;
		lastKnownPlayerPos = GameManager.inst.player.gridPosition;
		
		bool pAT = true;
		foreach (Enemy enemy in enemyList) {
			pAT &= enemy.TakePhaseAction();
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
		
		return pAT;
	}
	
	public void AddSubject(Enemy enemy) {
		enemyList.Add(enemy);
	}
	
	public bool HasPlayerMoved() {
		return playerPosLastTurn != GameManager.inst.player.gridPosition;
	}
	
	public List<Vector3Int> GetMovementOptions(Vector3Int fromPosition) {
		// since we call TakePhaseAction serially...
		// we don't need to know if an Enemy WILL move into a spot.
		// if they had higher priority, they will have already moved into it	
		// also, the conversion to HashSet, and the conversion back, is not worth it to remove from a list of spaces max
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
