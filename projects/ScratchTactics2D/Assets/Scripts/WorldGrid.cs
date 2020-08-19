using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class WorldGrid : MonoBehaviour
{
	private const int mapDimensionX = 5;
	private const int mapDimensionY = 5;
	private const int totalMapSize = mapDimensionX * mapDimensionY;
	private const int pixelsPerUnit = 32;	// in the future, get this from TileBase(?)
	
	private Tilemap baseTilemap;
	
	// invoke these dynamically to get the correct tile type. 0:grass, 1:dirt, etc. Simply add a new delegate when adding tiles
	private delegate Tile GetTileOption();
	private static List<GetTileOption> tileOptions = new List<GetTileOption>{
		TilesResourcesLoader.getGrassTile,
		TilesResourcesLoader.getDirtTile,
		TilesResourcesLoader.getWaterTile
	};
	
	public Vector3 Tile2RealPos(Vector3Int tilePos) {
		return baseTilemap.GetCellCenterWorld(tilePos);
	}
	
	public Vector3Int Real2TilePos(Vector3 realPos) {
		return baseTilemap.WorldToCell(realPos);
	}
	
	public Vector3 GetTileInDirection(Vector3 start, Vector3Int dirVector) {
		Vector3Int tileStart = Real2TilePos(start);
		Vector3Int tileEnd   = tileStart + dirVector;
		return Tile2RealPos(tileEnd);		
	}
	
	public void GenerateWorld() {
		// we have a Grid object which is actually attached
		// the Tilemap is a child of the Grid object
		baseTilemap = GetComponentsInChildren<Tilemap>()[0];
		
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
