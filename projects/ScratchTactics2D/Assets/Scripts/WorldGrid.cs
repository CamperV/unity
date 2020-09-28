using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class WorldGrid : MonoBehaviour
{
	public int mapDimensionX;
	public int mapDimensionY;	
	public Tilemap baseTilemap;

	private List<WorldTile> tileOptions;
	private Dictionary<Vector3Int, Component> occupancyGrid;
	private Dictionary<Vector3Int, WorldTile> worldTileGrid;
	private Dictionary<Enum.TileLevel, HashSet<Vector3Int>> currentSetTiles;	// @ level (int)
	
	public void Awake() {
		// we have a Grid object which is actually attached
		// the Tilemap is a child of the Grid object
		baseTilemap = GetComponentsInChildren<Tilemap>()[0];
		
		// tileOptions determine probability order as well
		// so when WorldGrid is generated, it will check if the tile is:
		//  grass first, then dirt, then water, then mountain
		tileOptions = new List<WorldTile>{
			ScriptableObject.CreateInstance<GrassWorldTile>() as GrassWorldTile,
			//ScriptableObject.CreateInstance<DirtWorldTile>() as DirtWorldTile,
			ScriptableObject.CreateInstance<WaterWorldTile>() as WaterWorldTile,
			ScriptableObject.CreateInstance<MountainWorldTile>() as MountainWorldTile
		};
		
		occupancyGrid   = new Dictionary<Vector3Int, Component>();
		worldTileGrid   = new Dictionary<Vector3Int, WorldTile>();
		currentSetTiles = new Dictionary<Enum.TileLevel, HashSet<Vector3Int>>() {
			[Enum.TileLevel.world] 	= new HashSet<Vector3Int>(),
			[Enum.TileLevel.overlay] = new HashSet<Vector3Int>(),
			[Enum.TileLevel.super] 	= new HashSet<Vector3Int>()
		};
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
	
	public Vector3Int RandomTileExceptType(HashSet<Type> except) {
		int x;
		int y;
		Vector3Int retVal;
		do {
			x = Random.Range(0, mapDimensionX);
			y = Random.Range(0, mapDimensionY);
			retVal = new Vector3Int(x, y, 0);
		} while (except.Contains(GetWorldTileAt(retVal).GetType()));
		return retVal;
	}
	
	public Vector3 RandomTileExceptTypeReal(HashSet<Type> except) {
		return Grid2RealPos(RandomTileExceptType(except));
	}
	
	public TileBase GetTileAt(Vector3Int tilePos) {
		return baseTilemap.GetTile(tilePos);
	}
	
	public WorldTile GetWorldTileAt(Vector3Int tilePos) {
		if (worldTileGrid.ContainsKey(tilePos)) {
			return worldTileGrid[tilePos];
		}
		return null;
	}
	
	public HashSet<Vector3Int> GetWorldTilePositions() {
		return new HashSet<Vector3Int>(worldTileGrid.Keys);
	}
	
	public void SetTile(Vector3Int tilePos, TileBase tile) {
		if (tile == null) {
			currentSetTiles[(Enum.TileLevel)tilePos.z].Remove(tilePos);
		} else {
			currentSetTiles[(Enum.TileLevel)tilePos.z].Add(tilePos);
		}
		baseTilemap.SetTile(tilePos, tile);
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
	
	public bool IsInBounds(Vector3Int tilePos) {
		return worldTileGrid.ContainsKey(tilePos);
	}
	
	public void HighlightTiles(HashSet<Vector3Int> tilePosSet, Color color) {
		foreach (var tilePos in tilePosSet) {
			TintTile(tilePos, color);
		}
	}
	
	public void ResetHighlightTiles(HashSet<Vector3Int> tilePosSet) {
		foreach (var tilePos in tilePosSet) {
			ResetTintTile(tilePos);
		}
	}
	
	public void TintTile(Vector3Int tilePos, Color color) {
		baseTilemap.SetTileFlags(tilePos, TileFlags.None);
		baseTilemap.SetColor(tilePos, color);
	}
	
	public void ResetTintTile(Vector3Int tilePos) {
		baseTilemap.SetTileFlags(tilePos, TileFlags.None);
		baseTilemap.SetColor(tilePos, new Color(1, 1, 1, 1));
	}
	
	public void OverlayAt(Vector3Int tilePos, OverlayTile tile) {
		Debug.Assert(tilePos.z == 0);
		Vector3Int overlayPos = new Vector3Int(tilePos.x, tilePos.y, (int)tile.level);
		SetTile(overlayPos, tile);
	}
	
	public void ResetOverlayAt(Vector3Int tilePos, Enum.TileLevel level) {
		Debug.Assert(tilePos.z == 0);
		Debug.Assert(level != Enum.TileLevel.world);
		Vector3Int overlayPos = new Vector3Int(tilePos.x, tilePos.y, (int)level);
		SetTile(overlayPos, null);
	}
	
	public void ClearTilesOnLevel(Enum.TileLevel level) {
		// can't use this to modify currentSetTiles during iteration
		foreach (Vector3Int tilePos in currentSetTiles[level]) {
			baseTilemap.SetTile(tilePos, null);
		}
		currentSetTiles[level].Clear();
	}
	
	public void GenerateWorld() {	
		baseTilemap.ClearAllTiles();
		int[,] mapMatrix = GenerateMapMatrix();
		
		ApplyMap(mapMatrix);
		
		// how many tiles do we want shown vertically?
		int yTiles = mapDimensionY;
		CameraManager.RefitCamera(yTiles);
		
		Vector2 minBounds = new Vector2(Mathf.Min(mapDimensionY+1, 10), (float)yTiles/2);
		Vector2 maxBounds = new Vector2(Mathf.Min(mapDimensionX-mapDimensionY-1, mapDimensionX-10), (float)yTiles/2);
		CameraManager.SetBounds(minBounds, maxBounds);
    }
	
	private int[,] GenerateMapMatrix() {
		int[,] mapMatrix = new int[mapDimensionX, mapDimensionY];
		
		for (int i = 0; i < mapMatrix.GetLength(0); i++) {
			for (int j = 0; j < mapMatrix.GetLength(1); j++) {
				// this determines which tile is chosen
				var rng = Random.Range(1, 7); // exclusive
				
				int selection;
				int probCounter = 0;
				for(selection = 0; selection < tileOptions.Count; selection++) {
					probCounter += tileOptions[selection].probability;
					if (rng <= probCounter) break;
				}
				Debug.Assert(probCounter <= 6);
				mapMatrix[i, j] = selection;
			}
		}
		return mapMatrix;
	}
	
	private void ApplyMap(int[,] mapMatrix) {
		var currentPos = baseTilemap.origin;

		for (int x = 0; x < mapMatrix.GetLength(0); x++) {
			for (int y = 0; y < mapMatrix.GetLength(1); y++) {
				// set the WorldTile in the actual tilemap		
				SetTile(currentPos, tileOptions[mapMatrix[x, y]]);
				
				// set in the WorldTile dictionary for easy path cost lookup
				worldTileGrid[new Vector3Int(x, y, 0)] = tileOptions[mapMatrix[x, y]];
				
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
