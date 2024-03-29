﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Extensions;

public static class BattleMapGenerator
{
	// Create bespoke terrain:terrain maps, and their "boarding pods"

	/////////////////////
	// TERRAIN:TERRAIN //
	/////////////////////
	// need two orientations for each (don't need to be identical)
	// Plain:Plain
	// Plain:Forest
	// Plain:Foothill
	// Plain:Road
	// 
	// Forest:Forest
	// Forest:Foothill
	// Forest:Road
	//
	// Foothill:Foothill
	// Foothill:Road
	//
	// Road:Road

	// Later:
	// Plain:Sand
	// Forest:Sand
	// Foothill:Sand
	// Sand:Road
	// Sand:Sand


	////////////////
	// CONTEXTUAL //
	////////////////
	// adjacent Mountains
	// adjacent Water

	////////////
	// UNIQUE //
	////////////
	// Fortress
	// Ruins
	// BanditCamps
	// Camps
	// Bridges(?)
	
	private static Dictionary<string, List<string>> prefabDesignators = new Dictionary<string, List<string>>{
		["Plain:Plain"]   = new List<string>{"PlainPlain_0"},
		
		["Plain:Forest"]  = new List<string>{"PlainForest_0"},
		["Forest:Plain"]  = new List<string>{"PlainForest_0"},

		["Forest:Forest"] = new List<string>{"ForestForest_0"},

		// SPECIAL MAPS
		["BossBanditCamp"] = new List<string>{"BossBanditCamp_0"}
	};

	private static Dictionary<string, List<string>> dockerDesignators = new Dictionary<string, List<string>>{
		["Plain:Plain"]   = new List<string>{"PlainDocker_0"},
		
		["Plain:Forest"]  = new List<string>{"PlainDocker_0"},
		["Forest:Plain"]  = new List<string>{"ForestDocker_0"},

		["Forest:Forest"] = new List<string>{"ForestDocker_0"}
	};

	public static List<BattleMap> GetMapsFromDesignator(string designator) {
		List<BattleMap> retVal = new List<BattleMap>();

		List<string> prefabDes = prefabDesignators.GetValueOtherwise(designator, new List<string>{"PlainPlain_0"});
		foreach (string tag in prefabDes) {
			BattleMap bmPrefab = Resources.Load<BattleMap>("Tilemaps/" + tag);
			retVal.Add(bmPrefab);
		}
		return retVal;
	}

	public static List<BattleMap> GetDockersFromDesignator(string designator) {
		List<BattleMap> retVal = new List<BattleMap>();

		List<string> prefabDes = dockerDesignators.GetValueOtherwise(designator, new List<string>{"PlainDocker_0"});
		foreach (string tag in prefabDes) {
			BattleMap bmPrefab = Resources.Load<BattleMap>("Tilemaps/" + tag);
			retVal.Add(bmPrefab);
		}
		return retVal;
	}

	public static void ApplyMap(BattleMap battleMap, Action<Vector3Int, TacticsTile> TileSetter) {
		foreach (Vector3Int tilePos in battleMap.Positions) {
			TileSetter(tilePos, battleMap.GetBattleMapTile(tilePos));
		}
	}

	// simply override with offset instead of Optional
	// this is because Vector3Int is not nullable, must be compile-time constant, generally not worth
	public static void ApplyMap(BattleMap battleMap, Action<Vector3Int, TacticsTile> TileSetter, Vector3Int offset) {
		foreach (Vector3Int tilePos in battleMap.Positions) {
			TileSetter(tilePos + offset, battleMap.GetBattleMapTile(tilePos));
		}
	}
}
