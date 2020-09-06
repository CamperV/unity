﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class WorldGrid : MonoBehaviour
{
	public int mapDimensionX;
	public int mapDimensionY;
	
	public Tilemap baseTilemap;
	
	// invoke these dynamically to get the correct tile type. 0:grass, 1:dirt, etc. Simply add a new delegate when adding tiles
	private delegate Tile GetTileOption();
	private static List<GetTileOption> tileOptions = new List<GetTileOption>{
		TilesResourcesLoader.GetGrassTile,
		TilesResourcesLoader.GetDirtTile,
		TilesResourcesLoader.GetWaterTile
	};
	private Dictionary<Vector3Int, Component> occupancyGrid;
	
	public void Awake() {
		// we have a Grid object which is actually attached
		// the Tilemap is a child of the Grid object
		baseTilemap = GetComponentsInChildren<Tilemap>()[0];
		occupancyGrid = new Dictionary<Vector3Int, Component>();
	}
	
	public Vector3 Grid2RealPos(Vector3Int tilePos) {
		return baseTilemap.GetCellCenterWorld(tilePos);
	}
	
	public Vector3Int Real2GridPos(Vector3 realPos) {
		return baseTilemap.WorldToCell(realPos);
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
	
	public TileBase GetTileAt(Vector3Int tilePos) {
		return baseTilemap.GetTile(tilePos);
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
	
	// neighbors are defined as adjacent squares in cardinal directions
	public List<Vector3Int> GetNeighbors(Vector3Int gridPosition) {
		List<Vector3Int> cardinal = new List<Vector3Int> {
			gridPosition + new Vector3Int( 0,  1, 0), // N
			gridPosition + new Vector3Int( 0, -1, 0), // S
			gridPosition + new Vector3Int(-1,  0, 0), // E
			gridPosition + new Vector3Int( 1,  0, 0)  // W
		};
		return cardinal;
	}
	
	public void HighlightTiles(HashSet<Vector3Int> tilePosSet) {
		foreach (var tilePos in tilePosSet) {
			TintTile(tilePos);
		}
	}
	
	public void ResetHighlightTiles(HashSet<Vector3Int> tilePosSet) {
		foreach (var tilePos in tilePosSet) {
			ResetTintTile(tilePos);
		}
	}
	
	public void TintTile(Vector3Int tilePos) {
		Color color = baseTilemap.GetColor(tilePos);
		
		baseTilemap.SetTileFlags(tilePos, TileFlags.None);
		baseTilemap.SetColor(tilePos, Color.red);
	}
	
	public void ResetTintTile(Vector3Int tilePos) {
		baseTilemap.SetTileFlags(tilePos, TileFlags.None);
		baseTilemap.SetColor(tilePos, new Color(1, 1, 1, 1));
	}
	
	public void GenerateWorld() {	
		baseTilemap.ClearAllTiles();
		int[,] mapMatrix = GenerateMapMatrix();
		
		ApplyMap(mapMatrix);
		CameraResizer.RefitCamera(new Vector3(baseTilemap.cellBounds.center.x,
											  baseTilemap.cellBounds.center.y,
											  -10),
								  mapDimensionY);
    }
	
	private int[,] GenerateMapMatrix() {
		int[,] mapMatrix = new int[mapDimensionX, mapDimensionY];
		
		for (int i = 0; i < mapMatrix.GetLength(0); i++) {
			for (int j = 0; j < mapMatrix.GetLength(1); j++) {
				mapMatrix[i, j] = Random.Range(0, tileOptions.Count);
			}
		}
		return mapMatrix;
	}
	
	private void ApplyMap(int[,] mapMatrix) {
		var currentPos = baseTilemap.origin;

		for (int x = 0; x < mapMatrix.GetLength(0); x++) {
			for (int y = 0; y < mapMatrix.GetLength(1); y++) {
				baseTilemap.SetTile(currentPos, tileOptions[mapMatrix[x, y]]());
				baseTilemap.SetTileFlags(currentPos, TileFlags.None);
				
				currentPos = new Vector3Int(currentPos.x,
											(int)(currentPos.y+baseTilemap.cellSize.y),
											currentPos.z);
			}
			currentPos = new Vector3Int((int)(currentPos.x+baseTilemap.cellSize.x),
										baseTilemap.origin.y,
										currentPos.z);
		}
		//
		baseTilemap.CompressBounds();
		baseTilemap.RefreshAllTiles();
	}
}
