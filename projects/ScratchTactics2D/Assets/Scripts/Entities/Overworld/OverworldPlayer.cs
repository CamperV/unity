using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

public class OverworldPlayer : OverworldEntity
{
	public override float moveSpeed { get => 1.0f; }
	public int moveThreshold { get => 200; }

	public override HashSet<Type> spawnable {
		get {
			return new HashSet<Type>() {
				typeof(VillageWorldTile),
				typeof(GrassWorldTile),
				typeof(ForestWorldTile),
				typeof(WaterWorldTile)
			};
		}
	}
	
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
		OverworldPlayer player = Instantiate(prefab);
		
		player.ResetPosition( GameManager.inst.worldGrid.RandomTileWithinType(player.spawnable) );
		GameManager.inst.worldGrid.UpdateOccupantAt(player.gridPosition, player);
		
		return player;
	}
	
	// action zone - these are called by a controller
	public override bool GridMove(int xdir, int ydir) {
		Vector3Int endTile = gridPosition.GridPosInDirection(GameManager.inst.worldGrid, new Vector2Int(xdir, ydir));

		// first check if you can overcome the cost of the tile at all	
		if (GameManager.inst.worldGrid.IsInBounds(endTile) && GameManager.inst.worldGrid.GetTileAt(endTile).cost <= moveThreshold) {
			return base.AttemptGridMove(xdir, ydir, GameManager.inst.worldGrid);
		} else {
			BumpTowards(endTile, GameManager.inst.worldGrid);
			return false;
		}
	}
	
	// this method is run when the Player moves INTO an Enemy
	// this will always create a battle, and never enter into an already in-progress one
	public override void OnBlocked<T>(T component) {
		OverworldEnemyBase hitEnemy = component as OverworldEnemyBase;
		hitEnemy.OnHit(); // play hit animation

		InitiateBattle(hitEnemy);
	}

	public void InitiateBattle(OverworldEnemyBase hitEnemy) {
		StartCoroutine(ExecuteAfterMoving(() => {
			// programmatically load in a TacticsGrid that matches what we need
			var playerTile = (WorldTile)GameManager.inst.worldGrid.GetTileAt(gridPosition);
			var enemyTile = (WorldTile)GameManager.inst.worldGrid.GetTileAt(hitEnemy.gridPosition);
		
			GameManager.inst.EnterBattleState();
			GameManager.inst.tacticsManager.CreateActiveBattle(this, hitEnemy, playerTile, enemyTile, Enum.Phase.player);
		}));
	}
}