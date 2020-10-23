using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TacticsGrid : GameGrid
{
	private Dictionary<Type, TacticsTile> tileOptions;
	private Dictionary<Vector3Int, Component> occupancyGrid;
	private Dictionary<Vector3Int, TacticsTile> tacticsTileGrid;
	
    void Awake() {
		base.Awake();
		
		// tileOptions determine probability order as well
		// so when Tactics is generated, it will check if the tile is:
		//  grass first, then dirt, then water, then mountain
		tileOptions = new Dictionary<Type, TacticsTile>() {
			[typeof(GrassWorldTile)] = ScriptableObject.CreateInstance<GrassIsoTile>() as GrassIsoTile,
			[typeof(RoadWorldTile)] = ScriptableObject.CreateInstance<GrassIsoTile>() as GrassIsoTile,
			[typeof(MountainWorldTile)] = ScriptableObject.CreateInstance<MountainIsoTile>() as MountainIsoTile
		};
		
		occupancyGrid = new Dictionary<Vector3Int, Component>();
		tacticsTileGrid = new Dictionary<Vector3Int, TacticsTile>();
	}
	
	public override bool IsInBounds(Vector3Int tilePos) {
		return tacticsTileGrid.ContainsKey(tilePos);
	}

    public void CreateTileMap(Vector3Int offset, WorldTile originTile) {
		var newOrigin = baseTilemap.origin + offset;

		for (int x = 0; x < originTile.battleGridSize.x; x++) {
			for (int y = 0; y < originTile.battleGridSize.y; y++) {
				tacticsTileGrid[newOrigin + new Vector3Int(x, y, 0)] = tileOptions[originTile.GetType()];
			}
		}
	}
	
	public void ApplyTileMap() {
		Debug.Assert(tacticsTileGrid.Count != 0);
		
		foreach(var pair in tacticsTileGrid.OrderBy(k => k.Key.x)) {
			baseTilemap.SetTile(pair.Key, pair.Value);
		}
		baseTilemap.CompressBounds();
		baseTilemap.RefreshAllTiles();
	}
	
	public Vector3Int GetDimensions() {
		return new Vector3Int(baseTilemap.cellBounds.xMax - baseTilemap.cellBounds.xMin,
							  baseTilemap.cellBounds.yMax - baseTilemap.cellBounds.yMin,
							  baseTilemap.cellBounds.zMax - baseTilemap.cellBounds.zMin);
	}
	
	public Vector3 GetGridCenterReal() {
		// interpolate the real world coordinates of the "middle" tiles
		Vector3Int floorCell = new Vector3Int((int)Mathf.Floor(baseTilemap.cellBounds.center.x-1),
											  (int)Mathf.Floor(baseTilemap.cellBounds.center.y-1), 0);
		Vector3Int ceilCell  = new Vector3Int((int)Mathf.Ceil(baseTilemap.cellBounds.center.x),
										   	  (int)Mathf.Ceil(baseTilemap.cellBounds.center.y), 0);
		return (baseTilemap.GetCellCenterWorld(floorCell) + baseTilemap.GetCellCenterWorld(ceilCell)) / 2.0f;
	}
	
	public Vector3 GetTilemapOrigin() {
		return baseTilemap.GetCellCenterWorld(baseTilemap.origin);
	}
}
