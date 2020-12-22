using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldPlayer : OverworldEntity
{
	public override float moveSpeed { get { return 1.0f; } }
	private Queue<Vector3Int> movementQueue;
	
	// abstract implementations
	public override List<string> defaultUnitTags {
		get {
			return new List<string>() {
				"UnitMercenary",
				"UnitMercenary",
				"UnitMercenary"
			};
		}
	}
	
	public static OverworldPlayer Spawn(OverworldPlayer prefab) {
		OverworldPlayer player = Instantiate(prefab, GameManager.inst.worldGrid.RandomTileReal(), Quaternion.identity);
		
		Vector3Int pos = new Vector3Int(1, (int)Mathf.Floor(GameManager.inst.worldGrid.mapDimensionY/2), 0);
		player.ResetPosition(pos);
		GameManager.inst.worldGrid.UpdateOccupantAt(player.gridPosition, player);
		
		return player;
	}

    protected override void Awake() {
		base.Awake();
		//
		movementQueue = new Queue<Vector3Int>();
    }
		
	public void ResetPosition(Vector3Int v) {
		gridPosition = v;
		transform.position = GameManager.inst.worldGrid.Grid2RealPos(gridPosition);
	}
	
	// action zone - these are called by a controller
	public override bool GridMove(int xdir, int ydir) {
		var success = base.AttemptGridMove(xdir, ydir, GameManager.inst.worldGrid);		
		return success;
	}
	
	// this method is run when the Player moves INTO an Enemy
	public override void OnBlocked<T>(T component) {
		OverworldEnemyBase hitEnemy = component as OverworldEnemyBase;
		hitEnemy.OnHit(); // play hit animation
		
		// programmatically load in a TacticsGrid that matches what we need
		var playerTile = (WorldTile)GameManager.inst.worldGrid.GetTileAt(gridPosition);
		var enemyTile = (WorldTile)GameManager.inst.worldGrid.GetTileAt(hitEnemy.gridPosition);
		
		GameManager.inst.EnterBattleState();
		var battleParticipants = new List<OverworldEntity>() { this, hitEnemy };
		var battleTiles = new List<WorldTile>(){ playerTile, enemyTile };
		GameManager.inst.tacticsManager.CreateActiveBattle(battleParticipants, battleTiles, Enum.Phase.player);
	}
}