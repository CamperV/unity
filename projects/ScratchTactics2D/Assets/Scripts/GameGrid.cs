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
	//
	
	protected void Awake() {				
		// we have a Grid object which is actually attached
		// the Tilemap is a child of the Grid object
		var tilemapComponents = GetComponentsInChildren<Tilemap>();
		baseTilemap    = tilemapComponents[0];
		depthTilemap   = tilemapComponents[1];
		overlayTilemap = tilemapComponents[2];
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
	
	public TileBase GetTileAt(Vector3Int tilePos) {
		return baseTilemap.GetTile(tilePos);
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
	
	public void SelectAt(Vector3Int tilePos, OverlayTile tile) {
		OverlayAt(tilePos, tile);
		StartCoroutine(FadeUp(overlayTilemap, tilePos));
	}
	
	public void ResetSelectionAt(Vector3Int tilePos) {
		// this will nullify the tilePos after fading
		StartCoroutine(FadeDownToNull(overlayTilemap, tilePos));
	}
	
	public IEnumerator FadeUp(Tilemap tilemap, Vector3Int tilePos) {
		tilemap.SetTileFlags(tilePos, TileFlags.None);
		float c = 0.0f;
		while (c < 1.0f) {
			tilemap.SetColor(tilePos, new Color(1, 1, 1, c));
			c += 0.025f;
			yield return null;
		}
	}
	
	public IEnumerator FadeDownToNull(Tilemap tilemap, Vector3Int tilePos) {
		tilemap.SetTileFlags(tilePos, TileFlags.None);
		float c = 1.0f;
		while (c > 0.0f) {
			tilemap.SetColor(tilePos, new Color(1, 1, 1, c));
			c -= 0.015f;
			yield return null;
		}
		tilemap.SetTile(tilePos, null);
	}
	
	// abstract zone
	public abstract bool IsInBounds(Vector3Int tilePos);
}
