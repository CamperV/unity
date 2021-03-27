using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

public abstract class GameGrid : MonoBehaviour
{
	public int mapDimensionX;
	public int mapDimensionY;
	
	[HideInInspector] public Tilemap baseTilemap;
	[HideInInspector] public Tilemap underlayTilemap;
	[HideInInspector] public Tilemap overlayTilemap;
	
	private Dictionary<Vector3Int, Component> occupancyGrid;
	protected Dictionary<Vector2Int, Vector3Int> translation2D;
	//
	
	protected virtual void Awake() {				
		// we have a Grid object which is actually attached
		// the Tilemap is a child of the Grid object
		var tilemapComponents = GetComponentsInChildren<Tilemap>();
		baseTilemap     = tilemapComponents[0];
		underlayTilemap = tilemapComponents[1];
		overlayTilemap  = tilemapComponents[2];

		occupancyGrid = new Dictionary<Vector3Int, Component>();
		translation2D = new Dictionary<Vector2Int, Vector3Int>();
	}
	
	// IMPORTANT: this is only used for converting locations to Unit/Entites positions
	// we need to account for the Z-shift here
	// wherever something needs to sit on top of a tile, but be sorted with them (TacticsEntities),
	// we need to add a small Z-offset
	public Vector3 Grid2RealPos(Vector3Int tilePos) {
		return baseTilemap.GetCellCenterWorld(tilePos) + new Vector3(0, 0, 1);
	}
	
	public virtual Vector3Int Real2GridPos(Vector3 realPos) {
		var toModify = baseTilemap.WorldToCell(realPos);
		return new Vector3Int(toModify.x, toModify.y, 0);
	}
		
	public Vector3Int RandomTile() {
		int x = Random.Range(0, mapDimensionX);
		int y = Random.Range(0, mapDimensionY);
		return new Vector3Int(x, y, 0);
	}
	
	public Vector3 RandomTileReal() {
		return Grid2RealPos(RandomTile());
	}
	
	public Vector3Int RandomTileWithin(HashSet<Vector3Int> within) {
		int x;
		int y;
		Vector3Int retVal;
		do {
			x = Random.Range(0, mapDimensionX);
			y = Random.Range(0, mapDimensionY);
			retVal = new Vector3Int(x, y, 0);
		} while (!within.Contains(retVal));
		return retVal;
	}
	
	public Vector3 RandomTileWithinReal(HashSet<Vector3Int> within) {
		return Grid2RealPos(RandomTileWithin(within));
	}
	
	// neighbors are defined as adjacent squares in cardinal directions
	public HashSet<Vector3Int> GetNeighbors(Vector3Int tilePos) {
		List<Vector2Int> cardinal = new List<Vector2Int> {
			Vector2Int.up, 	// N
			Vector2Int.right, // E
			Vector2Int.down, 	// S
			Vector2Int.left  	// W
		};
		
		HashSet<Vector3Int> retHash = new HashSet<Vector3Int>();
		foreach (Vector2Int cPos in cardinal) {
			Vector3Int pos = To3D(new Vector2Int(tilePos.x, tilePos.y) + cPos);
			if (IsInBounds(pos)) retHash.Add(pos);
		}
		return retHash;
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
	
	public virtual void SelectAt(Vector3Int tilePos, Color? color = null) {
		TintTile(baseTilemap, tilePos, color ?? Constants.selectColorBlue);
	}
	
	public virtual void ResetSelectionAt(Vector3Int tilePos, float fadeRate = 0.05f) {
		ResetTintTile(baseTilemap, tilePos);
	}

	// These exist because I am too lazy to figure out a better solution
	// many functions return a GameGrid, and this function should only be called on TacticsGrids
	// why virtual/override then? Stupid? Well, if I didn't do this, I'd have to go cast every
	// instance of GameGrid (from GetActiveGrid()) into a TacticsGrid
	// i'll fix it eventually... I hope
	public virtual void UnderlayAt(Vector3Int tilePos, Color color) { Debug.Assert(false); }
	public virtual void ResetUnderlayAt(Vector3Int tilePos) { Debug.Assert(false); }
	//

	public IEnumerator FadeUp(Tilemap tilemap, Vector3Int tilePos) {
		tilemap.SetTileFlags(tilePos, TileFlags.None);
		float c = 0.0f;
		while (c < 1.0f) {
			tilemap.SetColor(tilePos, new Color(1, 1, 1, c));
			c += 0.05f;
			yield return null;
		}
	}
	
	public IEnumerator FadeDownToNull(Tilemap tilemap, Vector3Int tilePos, float fadeRate) {
		tilemap.SetTileFlags(tilePos, TileFlags.None);
		float c = 1.0f;
		while (c > 0.0f) {
			tilemap.SetColor(tilePos, new Color(1, 1, 1, c));
			c -= fadeRate;
			yield return null;
		}
		tilemap.SetTile(tilePos, null);
	}
	
	public virtual void TintTile(Tilemap tilemap, Vector3Int tilePos, Color color) {
		if (tilemap.GetTile(tilePos) != null) {
			tilemap.SetTileFlags(tilePos, TileFlags.None);
			tilemap.SetColor(tilePos, color);
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

	public Vector3Int To3D(Vector2Int v) {
		if (translation2D.ContainsKey(v)) {
			return translation2D[v];
		} else {
			return new Vector3Int(v.x, v.y, 0);
		}
		
	}
	
	// abstract zone
	public abstract bool IsInBounds(Vector3Int tilePos);
	public abstract GameTile GetTileAt(Vector3Int tilePos);
	public abstract HashSet<Vector3Int> GetAllTilePos();
}
