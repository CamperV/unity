using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

public abstract class GameGrid : MonoBehaviour, IPathable
{
	[HideInInspector] public Tilemap baseTilemap;
	[HideInInspector] public Tilemap underlayTilemap;
	[HideInInspector] public Tilemap overlayTilemap;
	
	public Dictionary<Vector3Int, Component> occupancyGrid;
	//
	
	protected virtual void Awake() {				
		// we have a Grid object which is actually attached
		// the Tilemap is a child of the Grid object
		var tilemapComponents = GetComponentsInChildren<Tilemap>();
		baseTilemap     = tilemapComponents[0];
		underlayTilemap = tilemapComponents[1];
		overlayTilemap  = tilemapComponents[2];

		occupancyGrid = new Dictionary<Vector3Int, Component>();
	}

	// IPathable Definitions
	public virtual IEnumerable<Vector3Int> GetNeighbors(Vector3Int origin) {
        Vector3Int up = origin + Vector3Int.up;
        Vector3Int right = origin + Vector3Int.right;
        Vector3Int down = origin + Vector3Int.down;
        Vector3Int left = origin + Vector3Int.left;
        if (IsInBounds(up)) yield return up;
        if (IsInBounds(right)) yield return right;
        if (IsInBounds(down)) yield return down;
        if (IsInBounds(left)) yield return left;
    }
	public virtual int EdgeCost(Vector3Int src, Vector3Int dest) {
		return 1;
	}
	
	// IMPORTANT: this is only used for converting locations to Unit/Entities positions
	// we need to account for the Z-shift here
	// wherever something needs to sit on top of a tile, but be sorted with them (TacticsEntities),
	// we need to add a small Z-offset
	public virtual Vector3 Grid2RealPos(Vector3Int tilePos) {
		return baseTilemap.GetCellCenterWorld(tilePos);
	}
	
	public virtual Vector3Int Real2GridPos(Vector3 realPos) {
		var toModify = baseTilemap.WorldToCell(realPos);
		return new Vector3Int(toModify.x, toModify.y, 0);
	}

	public virtual Vector3Int To3D(Vector2Int v) {
		return new Vector3Int(v.x, v.y, 0);		
	}

	public void OverlayAt(Vector3Int tilePos, OverlayTile tile) {
		overlayTilemap.SetTile(tilePos, tile);
	}
	
	public void ResetOverlayAt(Vector3Int tilePos) {
		overlayTilemap.SetTile(tilePos, null);
	}

	public OverlayTile GetOverlayAt(Vector3Int tilePos) {
		return overlayTilemap.GetTile(tilePos) as OverlayTile;
	}

	public bool UnderlayNull(Vector3Int tilePos) {
		return underlayTilemap.GetTile(tilePos) == null;
	}

	// These exist because I am too lazy to figure out a better solution
	// many functions return a GameGrid, and this function should only be called on TacticsGrids
	// why virtual/override then? Stupid? Well, if I didn't do this, I'd have to go cast every
	// instance of GameGrid (from GetActiveGrid()) into a TacticsGrid
	// i'll fix it eventually... I hope
	public virtual void UnderlayAt(Vector3Int tilePos, Color color) { Debug.Assert(false); }
	public virtual void ResetUnderlayAt(Vector3Int tilePos) { Debug.Assert(false); }
	//

	// public IEnumerator FadeUp(Tilemap tilemap, Vector3Int tilePos) {
	// 	tilemap.SetTileFlags(tilePos, TileFlags.None);
	// 	float c = 0.0f;
	// 	while (c < 1.0f) {
	// 		tilemap.SetColor(tilePos, new Color(1, 1, 1, c));
	// 		c += 0.05f;
	// 		yield return null;
	// 	}
	// }
	
	// public IEnumerator FadeDownToNull(Tilemap tilemap, Vector3Int tilePos, float fadeRate) {
	// 	tilemap.SetTileFlags(tilePos, TileFlags.None);
	// 	float c = 1.0f;
	// 	while (c > 0.0f) {
	// 		tilemap.SetColor(tilePos, new Color(1, 1, 1, c));
	// 		c -= fadeRate;
	// 		yield return null;
	// 	}
	// 	tilemap.SetTile(tilePos, null);
	// }
	
	public virtual void TintTile(Tilemap tilemap, Vector3Int tilePos, Color color) {
		if (tilemap.GetTile(tilePos) != null) {
			tilemap.SetTileFlags(tilePos, TileFlags.None);
			tilemap.SetColor(tilePos, color);
			// tilemap.SetTileFlags(tilePos, TileFlags.LockColor);
			return;
		} else {
			Debug.Log("Not a valid Tint target");
			Debug.Assert(false);
		}
	}
	
	public virtual void ResetTintTile(Tilemap tilemap, Vector3Int tilePos) {
		tilemap.SetTileFlags(tilePos, TileFlags.None);
		tilemap.SetColor(tilePos, Color.white);
	}

	public virtual Color GetTint(Tilemap tilemap, Vector3Int tilePos) {
		return tilemap.GetColor(tilePos);
	}
	
	public Component OccupantAt(Vector3Int tilePos) {
		if (occupancyGrid.ContainsKey(tilePos)) {
			return occupancyGrid[tilePos];
		}
		return null;
	}
	
	public void UpdateOccupantAt(Vector3Int tilePos, Component newOccupant) {
		if (occupancyGrid.ContainsKey(tilePos)) {
			occupancyGrid[tilePos] = newOccupant;
		} else {
			occupancyGrid.Add(tilePos, newOccupant);
		}
	}
		
	public bool VacantAt(Vector3Int tilePos) {
		if (occupancyGrid.ContainsKey(tilePos)) {
			return occupancyGrid[tilePos] == null;
		} else {
			return true;
		}
	}
	
	public HashSet<Vector3Int> CurrentOccupantPositions<T>() {
		HashSet<Vector3Int> allPositions = new HashSet<Vector3Int>();
		foreach (Vector3Int k in occupancyGrid.Keys) {
			var occupantAt = OccupantAt(k);
			if (occupantAt != null && occupantAt.MatchesType(typeof(T))) {
				allPositions.Add(k);
			}
		}
		return allPositions;
	}

	public HashSet<Vector3Int> CurrentOccupantPositionsExcepting<T>() {
		HashSet<Vector3Int> allPositions = new HashSet<Vector3Int>();
		foreach (Vector3Int k in occupancyGrid.Keys) {
			var occupantAt = OccupantAt(k);
			if (occupantAt != null && !occupantAt.MatchesType(typeof(T))) {
				allPositions.Add(k);
			}
		}
		return allPositions;
	}

	public Dictionary<Vector3Int, T> GetTilemapDict<T>(Tilemap tilemap) where T : Tile {
		Dictionary<Vector3Int, T> retVal = new Dictionary<Vector3Int, T>();

		foreach (var pos in tilemap.cellBounds.allPositionsWithin) {
			Vector3Int v = new Vector3Int(pos.x, pos.y, pos.z);
			var tile = tilemap.GetTile<T>(v);
			if (tile != null) retVal[v] = tile;
		}
		return retVal;
	}

	public static IEnumerable<Vector3Int> GetPositions(Tilemap tilemap) {
		foreach (var pos in tilemap.cellBounds.allPositionsWithin) {
			Vector3Int v = new Vector3Int(pos.x, pos.y, pos.z);
			if (tilemap.HasTile(v)) yield return v;
		}
	}
	
	// abstract zone
	public abstract bool IsInBounds(Vector3Int tilePos);
	public abstract Vector3Int GetMouseToGridPos();
}
