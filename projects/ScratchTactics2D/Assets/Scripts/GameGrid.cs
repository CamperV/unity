using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public abstract class GameGrid : MonoBehaviour
{
	public int mapDimensionX;
	public int mapDimensionY;
	
	[HideInInspector] public Tilemap baseTilemap;
	[HideInInspector] public Tilemap depthTilemap;
	[HideInInspector] public Tilemap overlayTilemap;
	
	private Dictionary<Vector3Int, Component> occupancyGrid;
	//
	
	protected void Awake() {				
		// we have a Grid object which is actually attached
		// the Tilemap is a child of the Grid object
		var tilemapComponents = GetComponentsInChildren<Tilemap>();
		baseTilemap    = tilemapComponents[0];
		depthTilemap   = tilemapComponents[1];
		overlayTilemap = tilemapComponents[2];
		
		occupancyGrid = new Dictionary<Vector3Int, Component>();
	}
	
	public Vector3 Grid2RealPos(Vector3Int tilePos) {
		return baseTilemap.GetCellCenterWorld(tilePos);
	}
	
	public Vector3Int Real2GridPos(Vector3 realPos) {
		var toModify = baseTilemap.WorldToCell(realPos);
		return new Vector3Int(toModify.x, toModify.y, 0);
	}
	
	public Vector3 GetTileInDirection(Vector3 start, Vector3Int dirVector) {
		Vector3Int tileStart = Real2GridPos(start);
		Vector3Int tileEnd   = tileStart + dirVector;
		return Grid2RealPos(tileEnd);		
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
		List<Vector3Int> cardinal = new List<Vector3Int> {
			tilePos + Vector3Int.up, 	// N
			tilePos + Vector3Int.right, // E
			tilePos + Vector3Int.down, 	// S
			tilePos + Vector3Int.left  	// W
		};
		
		HashSet<Vector3Int> retHash = new HashSet<Vector3Int>();
		foreach (Vector3Int pos in cardinal) {
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
	
	public virtual void SelectAt(Vector3Int tilePos, Color? color = null) {
		TintTile(tilePos, color ?? Utils.selectColorBlue);
	}
	
	public virtual void ResetSelectionAt(Vector3Int tilePos, float fadeRate = 0.05f) {
		ResetTintTile(tilePos);
	}
	
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
	
	public void TintTile(Vector3Int tilePos, Color color) {
		if (baseTilemap.GetTile(tilePos) != null) {
			baseTilemap.SetTileFlags(tilePos, TileFlags.None);
			baseTilemap.SetColor(tilePos, color);
			return;
		} else if (depthTilemap.GetTile(tilePos) != null){
			depthTilemap.SetTileFlags(tilePos, TileFlags.None);
			depthTilemap.SetColor(tilePos, color);
			return;
		} else {
			Debug.Log("Not a valid Tint target");
			Debug.Assert(false);
		}
	}
	
	public void ResetTintTile(Vector3Int tilePos) {
		baseTilemap.SetTileFlags(tilePos, TileFlags.None);
		baseTilemap.SetColor(tilePos, Color.white);
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

	public List<Component> CurrentOccupants() {
		List<Component> allOccupants = new List<Component>();
		foreach (Vector3Int k in occupancyGrid.Keys) {
			var occupantAt = OccupantAt(k);
			if (occupantAt != null) {
				allOccupants.Add(occupantAt);
			}
		}
		return allOccupants;
	}
	
	public HashSet<Vector3Int> CurrentOccupantPositions<T>() {
		HashSet<Vector3Int> allPositions = new HashSet<Vector3Int>();
		foreach (Vector3Int k in occupancyGrid.Keys) {
			var occupantAt = OccupantAt(k);
			if (occupantAt != null && occupantAt.GetType() == typeof(T)) {
				allPositions.Add(k);
			}
		}
		return allPositions;
	}
	
	// abstract zone
	public abstract bool IsInBounds(Vector3Int tilePos);
	public abstract GameTile GetTileAt(Vector3Int tilePos);
	public abstract HashSet<Vector3Int> GetAllTilePos();
}
