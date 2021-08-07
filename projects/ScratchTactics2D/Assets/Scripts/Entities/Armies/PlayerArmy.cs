using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

public class PlayerArmy : Army
{
	// this cost is insurmountable to the PlayerArmy
	public int moveThreshold { get => Constants.standardTickCost*3; }
	
	// for moving around the Overworld
	// why in this file? Because it will be updated every time gridPosition is updated
	public virtual int visionRange { get => 6; }	// in tiles, includes the origin in radius

	private FieldOfView _fov;
	public FieldOfView fov {
		get => _fov;
		set {
			_fov = value;
			_fov.Display();
			GameManager.inst.overworld.UpdateVisibility(_fov);
		}
	}
	// for now, FieldOfView is only had by the PlayerArmy
	[HideInInspector] public override Vector3Int gridPosition {
		get => _gridPosition;

		// make sure you also update FOV when moving
		protected set {
			_gridPosition = value;
			fov = new FieldOfView(_gridPosition, visionRange);
		}
	}

	// abstract implementations
	public override List<string> defaultUnitTags {
		get {
			return new List<string>() {
				"KnightPlayerUnit",
				"ArcherPlayerUnit",
				"BanditPlayerUnit",
				"SpearmanPlayerUnit",
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
		StartCoroutine( spriteAnimator.ExecuteAfterMoving(() => {
			Terrain playerTerrain = GameManager.inst.overworld.TerrainAt(gridPosition);
			Terrain enemyTerrain = GameManager.inst.overworld.TerrainAt(combatant.gridPosition);
		
			GameManager.inst.EnterBattleState();
			GameManager.inst.tacticsManager.CreateActiveBattle(this, combatant, playerTerrain, enemyTerrain, Enum.Phase.player);
		}));
	}
}