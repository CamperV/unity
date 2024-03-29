﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using Extensions;
using Random = UnityEngine.Random;

public class PlayerArmy : Army, ITerrainAffectable, IFirstFrame
{
	public sealed override string armyTag { get => "PlayerArmy"; }

	public Color color_0 = new Color(0.85f, 0.79f, 1.00f);
	public Color color_1 = new Color(0.44f, 0.40f, 0.53f);
	public Color color_2 = new Color(0.26f, 0.24f, 0.32f);
	public Color color_3 = Color.black;
	private static Dictionary<string, Color> playerArmyDefaultPalette;

	// this cost is insurmountable to the PlayerArmy
	public static int moveThreshold { get => Constants.standardTickCost*3; }
	public static int visionRange = 6;	// in tiles, includes the origin in radius
	public static int startingFood = 150;

	public FieldOfView fov;
	
	public static PlayerArmy Spawn(PlayerArmy prefab) {
		PlayerArmy player = Instantiate(prefab);
		
		List<Vector3Int> villagePos = GameManager.inst.overworld.LocationsOf<Village>();
		int maxVillageY = villagePos.Max(t => t.y);
		player.UpdateGridPosition( villagePos.First(v => v.y == maxVillageY) , GameManager.inst.overworld);
		GameManager.inst.overworld.UpdateOccupantAt(player.gridPosition, player);

		return player;
	}

	void Awake() {
		playerArmyDefaultPalette = new Dictionary<string, Color>{
			["_BrightColor"] = color_0,
			["_MediumColor"] = color_1,
			["_DarkColor"]   = color_2,
			["_ShadowColor"] = color_3
		};
		GetComponent<PaletteSwapAndOutlineBehavior>().SetPalette(playerArmyDefaultPalette);

		List<string> startingUnitClasses = new List<string>{
			"KnightClass",
			"SoldierClass",
			"ArcherClass", "ArcherClass"
		};

		// generate your units here (name, tags, etc)
		PopulateBarracksFromTags(startingUnitClasses);
	}

	// IFirstFrame
	public void OnFirstFrame() {
		GlobalPlayerState.SetFood(startingFood);
		UpdateFOV();
	}

	public void UpdateFOV() {
		fov = new FieldOfView(gridPosition, visionRange);
		foreach (Vector3Int pos in fov.field.Keys) {
			GlobalPlayerState.UpdateRevealedPositions(pos);
		}
		fov.Display();

		// this updates enemy army's visibility
		// we don't do it in this class because there can be other objects that have separate FOVs
		GameManager.inst.overworld.UpdateVisibility(fov);
	}
	
	// action zone - these are called by a controller
	public override bool GridMove(int xdir, int ydir) {
		var overworld = GameManager.inst.overworld;

		// first check if you can overcome the cost of the tile at all and see if there is anybody there already
		Vector3Int endPos = gridPosition.GridPosInDirection(overworld, new Vector2Int(xdir, ydir));

		Component occupant;
		bool success = AttemptGridMove(xdir, ydir, overworld, out occupant, addlConditions: overworld.TerrainAt(endPos).tickCost <= moveThreshold);

		// if you're able to move, apply the terrain effects first
		if (success) {
			OnEnterTerrain(overworld.TerrainAt(endPos));
			UpdateFOV();
		}

		// if you bump into something:
		if (occupant?.MatchesType(typeof(EnemyArmy)) ?? false) {
			EnemyArmy enemy = occupant as EnemyArmy;
			enemy.OnHit();
			InitiateBattle(enemy);
		}
		return occupant == null && success;
	}
	
	// ITerrainAffectable
	public void OnEnterTerrain(Terrain terrain) {
		Debug.Log($"Player entered terrain {terrain}");
		terrain.ApplyTerrainEffect(this);
		
		GlobalPlayerState.UpdateFood(-terrain.foodCost);
	}

	public void InitiateBattle(EnemyArmy combatant) {
		combatant.state = Enum.EnemyArmyState.inBattle;

		StartCoroutine( spriteAnimator.ExecuteAfterMoving(() => {
			Terrain playerTerrain = GameManager.inst.overworld.TerrainAt(gridPosition);
			Terrain enemyTerrain = GameManager.inst.overworld.TerrainAt(combatant.gridPosition);
		
			GameManager.inst.EnterBattleState();
			GameManager.inst.overworld.turnManager.Suspend();
			Battle.CreateActiveBattle(this, combatant, playerTerrain, enemyTerrain, Enum.Phase.player);
		}));
	}

	// Unit things:
	public override void PopulateBarracksFromTags(List<string> pod) {
		foreach (string unitClassTag in pod) {
			Guid _ID = Guid.NewGuid();
			string _unitName = $"{unitClassTag} Player!Jeremy {Random.Range(0, 101)}";
			UnitState defaultState = UnitClass.GenerateDefaultState(_ID, _unitName, unitClassTag);

			// now save the unit in our barracks
			EnlistUnit(defaultState);
		}
	}
}