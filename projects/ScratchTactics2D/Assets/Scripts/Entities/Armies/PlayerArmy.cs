using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

public class PlayerArmy : Army
{
	public int moveThreshold { get => Constants.standardTickCost*5; }
	
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
	
	public static PlayerArmy Spawn(PlayerArmy prefab) {
		PlayerArmy player = Instantiate(prefab);
		
		HashSet<Type> canSpawnInto = new HashSet<Type>(){ typeof(Village) };
		player.ResetPosition( GameManager.inst.overworld.RandomTileWithinType(canSpawnInto) );
		GameManager.inst.overworld.UpdateOccupantAt(player.gridPosition, player);
		
		return player;
	}
	
	// action zone - these are called by a controller
	public override bool GridMove(int xdir, int ydir) {
		var overworld = GameManager.inst.overworld;

		// first check if you can overcome the cost of the tile at all and see if there is anybody there already
		Vector3Int endPos = gridPosition.GridPosInDirection(overworld, new Vector2Int(xdir, ydir));
		Component occupant = AttemptGridMove(xdir, ydir, overworld, addlConditions: overworld.TerrainAt(endPos).tickCost <= moveThreshold);

		if (occupant?.MatchesType(typeof(EnemyArmy)) ?? false) {
			EnemyArmy enemy = occupant as EnemyArmy;
			enemy.OnHit();
			InitiateBattle(enemy);
		}
		return occupant == null;
	}

	public void InitiateBattle(EnemyArmy combatant) {
		StartCoroutine(ExecuteAfterMoving(() => {
			// programmatically load in a TacticsGrid that matches what we need
			Terrain playerTerrain = GameManager.inst.overworld.TerrainAt(gridPosition);
			Terrain enemyTerrain = GameManager.inst.overworld.TerrainAt(combatant.gridPosition);
		
			GameManager.inst.EnterBattleState();
			GameManager.inst.tacticsManager.CreateActiveBattle(this, combatant, playerTerrain, enemyTerrain, Enum.Phase.player);
		}));
	}
}