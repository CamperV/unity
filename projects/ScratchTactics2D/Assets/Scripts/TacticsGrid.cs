using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TacticsGrid : MonoBehaviour
{
	public int mapDimensionX;
	public int mapDimensionY;
	private Tilemap baseTilemap;
	
	private Dictionary<Type, TacticsTile> tileOptions;
	private Dictionary<Vector3Int, Component> occupancyGrid;
	private Dictionary<Vector3Int, TacticsTile> tacticsTileGrid;
	
    public void Awake() {
		Debug.Log("TacticsGrid: I live!");
		// we have a Grid object which is actually attached
		// the Tilemap is a child of the Grid object
		baseTilemap = GetComponentsInChildren<Tilemap>()[0];
		
		// tileOptions determine probability order as well
		// so when Tactics is generated, it will check if the tile is:
		//  grass first, then dirt, then water, then mountain
		tileOptions = new Dictionary<Type, TacticsTile>() {
			[typeof(GrassWorldTile)] = ScriptableObject.CreateInstance<GrassIsoTile>() as GrassIsoTile,
			[typeof(MountainWorldTile)] = ScriptableObject.CreateInstance<MountainIsoTile>() as MountainIsoTile
		};
		
		occupancyGrid = new Dictionary<Vector3Int, Component>();
		tacticsTileGrid = new Dictionary<Vector3Int, TacticsTile>();
	}

    public void CreateTacticsMap(Vector3Int offset, Type fromTileType) {
		var newOrigin = baseTilemap.origin + offset;

		for (int x = 0; x < mapDimensionX; x++) {
			for (int y = 0; y < mapDimensionY; y++) {
				baseTilemap.SetTile(newOrigin + new Vector3Int(x, y, 0), tileOptions[fromTileType]);
			}
		}
		//
		baseTilemap.CompressBounds();
		baseTilemap.RefreshAllTiles();
	}
}
