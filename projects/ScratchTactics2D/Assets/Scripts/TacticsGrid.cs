using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TacticsGrid : GameGrid
{
	private Dictionary<Type, TacticsTile> tileOptions;
	private Dictionary<Vector3Int, TacticsTile> tacticsTileGrid;

	private OverlayTile waypointOverlayTile;
	
    protected void Awake() {
		base.Awake();
		
		// tileOptions determine probability order as well
		// so when Tactics is generated, it will check if the tile is:
		//  grass first, then dirt, then water, then mountain
		tileOptions = new Dictionary<Type, TacticsTile>() {
			[typeof(GrassWorldTile)] = (ScriptableObject.CreateInstance<GrassIsoTile>() as GrassIsoTile),
			[typeof(RoadWorldTile)] = (ScriptableObject.CreateInstance<GrassIsoTile>() as GrassIsoTile),
			[typeof(WaterWorldTile)] = (ScriptableObject.CreateInstance<GrassIsoTile>() as GrassIsoTile),
			[typeof(WaterRoadWorldTile)] = (ScriptableObject.CreateInstance<GrassIsoTile>() as GrassIsoTile),
			[typeof(ForestWorldTile)] = (ScriptableObject.CreateInstance<ForestIsoTile>() as ForestIsoTile),
			[typeof(ForestRoadWorldTile)] = (ScriptableObject.CreateInstance<ForestIsoTile>() as ForestIsoTile),
			[typeof(MountainWorldTile)] = (ScriptableObject.CreateInstance<MountainIsoTile>() as MountainIsoTile),
			[typeof(MountainRoadWorldTile)] = (ScriptableObject.CreateInstance<MountainIsoTile>() as MountainIsoTile)
		};
		
		tacticsTileGrid = new Dictionary<Vector3Int, TacticsTile>();

		waypointOverlayTile = PathOverlayIsoTile.GetTileWithSprite(1);
	}
	
	public override bool IsInBounds(Vector3Int tilePos) {
		return tacticsTileGrid.ContainsKey(tilePos);
	}
	
	public override GameTile GetTileAt(Vector3Int tilePos) {
		if (tacticsTileGrid.ContainsKey(tilePos)) {
			return tacticsTileGrid[tilePos];
		}
		return null;
	}

	public override HashSet<Vector3Int> GetAllTilePos() {
		return new HashSet<Vector3Int>(tacticsTileGrid.Keys);
	}
	//
	// END OVERRIDE ZONE
	//

	public void SelectAtAlternate(Vector3Int tilePos) {
		OverlayAt(tilePos, waypointOverlayTile);
	}

	public virtual void ResetSelectionAtAlternate(Vector3Int tilePos, float fadeRate = 0.025f) {
		ResetOverlayAt(tilePos);
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
