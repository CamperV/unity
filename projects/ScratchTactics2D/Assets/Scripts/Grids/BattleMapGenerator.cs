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

		["Forest:Forest"] = new List<string>{"ForestForest_0"}
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

	// orientation here refers to the player
	// i.e., an orientation of Vector3Int.down implies the following: (Enemy v Player)
	//		___
	//	   | E |
	//	    ---
	//	   | P |
	//      ---
	public static Zone GetZoneFromOrientation(BattleMap battleMap, Vector3Int orientation) {
		List<Vector3Int> positions = new List<Vector3Int>();

		// get either spawn zone 1 (near) or spawn zone 2 (far)
		// transforming E/W is done outside this function
		if (orientation == Vector3Int.down || orientation == Vector3Int.left) {
			positions = battleMap.GetPositionsOfType<SpawnMarkerTacticsTile>().ToList();
		} else if (orientation == Vector3Int.up || orientation == Vector3Int.right) {
			positions = battleMap.GetPositionsOfType<SpawnMarkerTacticsTile>().ToList();
		}
		return new Zone(positions);
	}

	public static void ApplyMap(BattleMap battleMap, Action<Vector3Int, TacticsTile> TileSetter) {
		foreach (Vector3Int tilePos in battleMap.Positions) {
			TileSetter(tilePos, battleMap.GetTileAt(tilePos));
		}
	}
}
