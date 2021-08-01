using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class BattleMap : MonoBehaviour
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

	private Tilemap baseTilemap;
	private Tilemap overlayTilemap;

	void Awake() {
		baseTilemap = GetComponent<Tilemap>();
		overlayTilemap = GetComponentsInChildren<Tilemap>()[0];
	}

	private static Dictionary<string, List<string>> prefabDesignators = new Dictionary<string, List<string>>{
		["Plain:Plain"]   = new List<string>{"PlainPlain_0"},
		["Plain:Forest"]  = new List<string>{"PlainForest_0"},

		["Forest:Forest"] = new List<string>{"ForestForest_0"}
	};

	public static List<BattleMap> GetMapsFromDesignator(string designator) {
		List<BattleMap> retVal = new List<BattleMap>();

		// note that we need to Instantiate to get certain fields from the Components
		// otherwise the GO is marked as inactive and we can't query it
		if (prefabDesignators.ContainsKey(designator)) {
			foreach (string tag in prefabDesignators[designator]) {
				BattleMap bmPrefab = Resources.Load<BattleMap>("Tilemaps/" + tag);
				retVal.Add( Instantiate(bmPrefab) );
			}
		}

		return retVal;
	}

	public List<Vector3Int> GetSpawnLocations() {
		// get all set tiles in the tilemap
		// determine their types, and return the appropriate ones
		return new List<Vector3Int>();
	}
}
