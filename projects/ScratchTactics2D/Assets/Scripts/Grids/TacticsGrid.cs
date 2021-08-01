using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class TacticsGrid : GameGrid
{
	public int mapDimensionX;
	public int mapDimensionY;
	
	private Dictionary<Vector3Int, TacticsTile> tacticsTileGrid;
	private Grid baseGrid;

	private OverlayTile waypointOverlayTile;
	private OverlayTile selectionOverlayTile;
	
    protected override void Awake() {
		base.Awake();

		tacticsTileGrid = new Dictionary<Vector3Int, TacticsTile>();
		baseGrid = GetComponent<Grid>();

		waypointOverlayTile = PathOverlayIsoTile.GetTileWithSprite(1);
		selectionOverlayTile = ScriptableObject.CreateInstance<SelectOverlayIsoTile>() as SelectOverlayIsoTile;
	}

	// IPathable definitions
	public override IEnumerable<Vector3Int> GetNeighbors(Vector3Int tilePos) {
		List<Vector2Int> cardinal = new List<Vector2Int> {
			Vector2Int.up, 		// N
			Vector2Int.right, 	// E
			Vector2Int.down, 	// S
			Vector2Int.left  	// W
		};
		
		foreach (Vector2Int cPos in cardinal) {
			Vector3Int pos = To3D(new Vector2Int(tilePos.x, tilePos.y) + cPos);
			if (IsInBounds(pos)) yield return pos;
		}
	}
	public override int EdgeCost(Vector3Int src, Vector3Int dest) {
        return 1;
    }
	
	public override bool IsInBounds(Vector3Int tilePos) {
		return tacticsTileGrid.ContainsKey(tilePos);
	}
	
	public override Tile GetTileAt(Vector3Int tilePos) {
		if (tacticsTileGrid.ContainsKey(tilePos)) {
			return tacticsTileGrid[tilePos];
		}
		return null;
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
		underlayTilemap.SetTile(tilePos, selectionOverlayTile);
		TintTile(underlayTilemap, tilePos, color);
	}

	public override void ResetUnderlayAt(Vector3Int tilePos) {
		underlayTilemap.SetTile(tilePos, null);
		ResetTintTile(underlayTilemap, tilePos);
	}

    public void CreateDominoTileMap(Vector3Int offset, Terrain originTerrain) {
		var newOrigin = baseTilemap.origin + offset;

		for (int x = 0; x < originTerrain.battleGridSize.x; x++) {
			for (int y = 0; y < originTerrain.battleGridSize.y; y++) {
				Vector3Int pos = newOrigin + new Vector3Int(x, y, Random.Range(0, 2));
				tacticsTileGrid[pos] = originTerrain.tacticsTile;
				translation2D[new Vector2Int(pos.x, pos.y)] = pos;
			}
		}
	}
	
	public void ApplyTileMap(bool noCompress = false) {
		Debug.Assert(tacticsTileGrid.Count != 0);
		
		foreach(var pair in tacticsTileGrid.OrderBy(k => k.Key.x)) {
			baseTilemap.SetTile(pair.Key, pair.Value);
		}

		if (noCompress) return;
		baseTilemap.CompressBounds();
		baseTilemap.RefreshAllTiles();

		// after refreshing all tiles, set rudimentary shading
		float tintScale = 0.20f;
		int maxZ = tacticsTileGrid.Keys.Max(it => it.z);
		foreach(var pair in tacticsTileGrid) {
			// tint the lower tiles by a step value, to create some shade
			// and fill out the tiles all the way to the bottom (of the screen, so like 10 for now)
			for (int i = pair.Key.z; i > -3; i--) {
				Vector3Int lower = new Vector3Int(pair.Key.x, pair.Key.y, i);
				baseTilemap.SetTile(lower, pair.Value);

				baseTilemap.SetTileFlags(lower, TileFlags.None);
				float tint = 1.0f - (tintScale * (maxZ - i));
				//float a = (i > -3) ? 1.0f : 1.0f - ( (tintScale/2.0f) * (-3 - i));
				float a = 1.0f;
				baseTilemap.SetColor(lower, new Color(tint, tint, tint, a));
			}
		}
	}
	
	public Vector3Int GetDimensions() {
		return new Vector3Int(baseTilemap.cellBounds.xMax - baseTilemap.cellBounds.xMin,
							  baseTilemap.cellBounds.yMax - baseTilemap.cellBounds.yMin,
							  baseTilemap.cellBounds.zMax - baseTilemap.cellBounds.zMin);
	}

	public Vector3 GetCellRadius2D() {
		Vector3Int maxCell = new Vector3Int(baseTilemap.cellBounds.xMax, baseTilemap.cellBounds.yMax, 0);
		float mag = (baseTilemap.GetCellCenterWorld(maxCell) - GetGridCenterReal()).magnitude;
		return new Vector3(0, mag, 0);
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

	public List<Vector3Int> GetSurfacePositions() {
		return translation2D.Values.ToList();
	}

	public Bounds GetBounds() {
		return baseTilemap.localBounds;
	}
}
