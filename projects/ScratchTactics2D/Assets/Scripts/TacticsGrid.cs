using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class TacticsGrid : GameGrid
{
	private Dictionary<Type, TacticsTile> tileOptions;
	private Dictionary<Vector3Int, TacticsTile> tacticsTileGrid;

	private Grid baseGrid;

	private OverlayTile waypointOverlayTile;
	private OverlayTile selectionOverlayTile;
	
    protected override void Awake() {
		base.Awake();
		
		// tileOptions determine probability order as well
		// so when Tactics is generated, it will check if the tile is:
		//  grass first, then dirt, then water, then mountain
		tileOptions = new Dictionary<Type, TacticsTile>() {
			[typeof(GrassWorldTile)] = (ScriptableObject.CreateInstance<GrassIsoTile>() as GrassIsoTile),
			[typeof(RoadWorldTile)] = (ScriptableObject.CreateInstance<GrassIsoTile>() as GrassIsoTile),
			//
			[typeof(WaterWorldTile)] = (ScriptableObject.CreateInstance<WaterIsoTile>() as WaterIsoTile),
			[typeof(WaterRoadWorldTile)] = (ScriptableObject.CreateInstance<WaterIsoTile>() as WaterIsoTile),
			//
			[typeof(ForestWorldTile)] = (ScriptableObject.CreateInstance<ForestIsoTile>() as ForestIsoTile),
			[typeof(ForestRoadWorldTile)] = (ScriptableObject.CreateInstance<ForestIsoTile>() as ForestIsoTile),
			//
			[typeof(MountainWorldTile)] = (ScriptableObject.CreateInstance<MountainIsoTile>() as MountainIsoTile),
			[typeof(MountainRoadWorldTile)] = (ScriptableObject.CreateInstance<MountainIsoTile>() as MountainIsoTile)
		};
		
		tacticsTileGrid = new Dictionary<Vector3Int, TacticsTile>();
		baseGrid = GetComponent<Grid>();

		waypointOverlayTile = PathOverlayIsoTile.GetTileWithSprite(1);
		selectionOverlayTile = ScriptableObject.CreateInstance<SelectOverlayIsoTile>() as SelectOverlayIsoTile;
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
	
	public override Vector3Int Real2GridPos(Vector3 realPos) {
		return baseGrid.WorldToCell(realPos);
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

	public override void UnderlayAt(Vector3Int tilePos, Color color) {
		//var overlayPosition = new Vector3Int(tilePos.x, tilePos.y, 1);
		//baseTilemap.SetTile(overlayPosition, selectionOverlayTile);
		//TintTile(overlayPosition, color);
		TintTile(tilePos, color);
	}

	public override void ResetUnderlayAt(Vector3Int tilePos) {
		//var overlayPosition = new Vector3Int(tilePos.x, tilePos.y, 1);
		//baseTilemap.SetTile(overlayPosition, null);
		//ResetTintTile(overlayPosition);
		ResetTintTile(tilePos);
	}

    public void CreateTileMap(Vector3Int offset, WorldTile originTile) {
		var newOrigin = baseTilemap.origin + offset;

		for (int x = 0; x < originTile.battleGridSize.x; x++) {
			for (int y = 0; y < originTile.battleGridSize.y; y++) {
				int zval = Random.Range(0, 2);
				tacticsTileGrid[newOrigin + new Vector3Int(x, y, zval)] = tileOptions[originTile.GetType()];
			}
		}
	}
	
	public void ApplyTileMap(bool noCompress = false) {
		Debug.Assert(tacticsTileGrid.Count != 0);
		
		foreach(var pair in tacticsTileGrid.OrderBy(k => k.Key.x)) {
			Debug.Log($"setting {pair.Key}");
			baseTilemap.SetTile(pair.Key, pair.Value);
		}

		if (noCompress) return;
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
