using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

public class OverworldPlayer : Army
{
	public override float moveSpeed { get => 1.0f; }
	public int moveThreshold { get => Constants.standardTickCost*5; }

	public override HashSet<Type> spawnable {
		get {
			return new HashSet<Type>() {
				typeof(Village)
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
		
		player.ResetPosition( GameManager.inst.overworld.RandomTileWithinType(player.spawnable) );
		GameManager.inst.overworld.UpdateOccupantAt(player.gridPosition, player);
		
		return player;
	}
	
	// action zone - these are called by a controller
	public override bool GridMove(int xdir, int ydir) {
		var overworld = GameManager.inst.overworld;

		// first check if you can overcome the cost of the tile at all and see if there is anybody there already
		Vector3Int endPos = gridPosition.GridPosInDirection(overworld, new Vector2Int(xdir, ydir));
		Component occupant = AttemptGridMove(xdir, ydir, overworld, addlConditions: overworld.TerrainAt(endPos).tickCost <= moveThreshold);

		if (occupant?.MatchesType(typeof(OverworldEnemyBase)) ?? false) {
			OverworldEnemyBase enemy = occupant as OverworldEnemyBase;
			enemy.OnHit();
			InitiateBattle(enemy);
		}
		return occupant == null;
	}

	public void InitiateBattle(OverworldEnemyBase combatant) {
		StartCoroutine(ExecuteAfterMoving(() => {
			// programmatically load in a TacticsGrid that matches what we need
			Terrain playerTerrain = GameManager.inst.overworld.TerrainAt(gridPosition);
			Terrain enemyTerrain = GameManager.inst.overworld.TerrainAt(combatant.gridPosition);
		
			GameManager.inst.EnterBattleState();
			GameManager.inst.tacticsManager.CreateActiveBattle(this, combatant, playerTerrain, enemyTerrain, Enum.Phase.player);
		}));
	}
}