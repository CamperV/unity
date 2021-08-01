using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

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

		["Forest:Forest"] = new List<string>{"ForestForest_0"}
	};

	private enum TacticsTileEnum {
		water,
		grass, grassRough, grassRock,
		forest, forestRough, forestTree,
		mountain
	};

	private static TacticsTile[] tileOptions = new TacticsTile[]{
		ScriptableObject.CreateInstance<WaterTacticsTile>(),
		//
		ScriptableObject.CreateInstance<GrassTacticsTile>(),
		ScriptableObject.CreateInstance<GrassRoughTacticsTile>(),
		ScriptableObject.CreateInstance<GrassRockTacticsTile>(),
		//
		ScriptableObject.CreateInstance<ForestTacticsTile>(),
		ScriptableObject.CreateInstance<ForestBrushTacticsTile>(),
		ScriptableObject.CreateInstance<ForestTreeTacticsTile>(),
		//
		ScriptableObject.CreateInstance<MountainTacticsTile>()
	};
	
	private static TacticsTile TileOption(TacticsTileEnum tileType) {
		return tileOptions[(int)tileType];
	}

	public static List<BattleMap> GetMapsFromDesignator(string designator) {
		List<BattleMap> retVal = new List<BattleMap>();

		Debug.Log($"Querying desig {designator}");
		if (prefabDesignators.ContainsKey(designator)) {
			foreach (string tag in prefabDesignators[designator]) {
				BattleMap bmPrefab = Resources.Load<BattleMap>("Tilemaps/" + tag);
				retVal.Add(bmPrefab);
			}
		} else {
			List<string> des = new List<string>{"PlainPlain_0"};
			foreach (string tag in des) {
				BattleMap bmPrefab = Resources.Load<BattleMap>("Tilemaps/" + tag);
				retVal.Add(bmPrefab);
			}
		}
		return retVal;
	}

	public static void ApplyMap(BattleMap battleMap, Action<Vector3Int, TacticsTile> TileSetter) {
		foreach (Vector3Int tilePos in battleMap.Positions) {
			TileSetter(tilePos, battleMap.GetTileAt(tilePos));
		}
	}
}
