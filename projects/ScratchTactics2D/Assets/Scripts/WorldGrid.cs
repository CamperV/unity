using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class WorldGrid : GameGrid
{
	private List<WorldTile> tileOptions;
	private OverlayTile selectTile;
	private Dictionary<Vector3Int, WorldTile> worldTileGrid;
	
	private Canvas tintCanvas;
	
	void Awake() {
		base.Awake();
		
		// tileOptions determine probability order as well
		// so when WorldGrid is generated, it will check if the tile is:
		//  grass first, then dirt, then water, then mountain
		tileOptions = new List<WorldTile>{
			ScriptableObject.CreateInstance<GrassWorldTile>() as GrassWorldTile,
			ScriptableObject.CreateInstance<DirtWorldTile>() as DirtWorldTile,
			ScriptableObject.CreateInstance<WaterWorldTile>() as WaterWorldTile,
			ScriptableObject.CreateInstance<MountainWorldTile>() as MountainWorldTile
		};
		selectTile = ScriptableObject.CreateInstance<SelectOverlayTile>() as SelectOverlayTile;
		
		worldTileGrid = new Dictionary<Vector3Int, WorldTile>();
		
		// set camera and canvas
		tintCanvas = GetComponentsInChildren<Canvas>()[0];
		tintCanvas.renderMode = RenderMode.ScreenSpaceCamera;
		tintCanvas.worldCamera = Camera.main;
		tintCanvas.sortingLayerName = "Overworld Entities";
		tintCanvas.sortingOrder = 1;
	}

	public override bool IsInBounds(Vector3Int tilePos) {
		return worldTileGrid.ContainsKey(tilePos);
	}
		
	public override GameTile GetTileAt(Vector3Int tilePos) {
		if (worldTileGrid.ContainsKey(tilePos)) {
			return worldTileGrid[tilePos];
		}
		return null;
	}

	public override HashSet<Vector3Int> GetAllTilePos() {
		return new HashSet<Vector3Int>(worldTileGrid.Keys);
	}

	public override void SelectAt(Vector3Int tilePos, Color? color = null) {
		OverlayAt(tilePos, selectTile);
		StartCoroutine(FadeUp(overlayTilemap, tilePos));
	}
	
	public override void ResetSelectionAt(Vector3Int tilePos, float fadeRate = 0.10f) {
		// this will nullify the tilePos after fading
		StartCoroutine(FadeDownToNull(overlayTilemap, tilePos, fadeRate));
	}
	
	public void EnableTint() {
		tintCanvas.gameObject.SetActive(true);
	}
	
	public void DisableTint() {
		tintCanvas.gameObject.SetActive(false);
	}
	
	public void SetAppropriateTile(Vector3Int tilePos, WorldTile tile) {	
		// set in the WorldTile dictionary for easy path cost lookup
		worldTileGrid[tilePos] = tile;
		
		switch (tile.depth) {
			case 0:			
				baseTilemap.SetTile(tilePos, tile);
				break;
			case 1:			
				depthTilemap.SetTile(tilePos, tile);
				break;
		}
	}
	
	public Vector3Int RandomTileExceptType(HashSet<Type> except) {
		int x;
		int y;
		Vector3Int retVal;
		do {
			x = Random.Range(0, mapDimensionX);
			y = Random.Range(0, mapDimensionY);
			retVal = new Vector3Int(x, y, 0);
		} while (except.Contains(GetTileAt(retVal).GetType()));
		return retVal;
	}
	
	public Vector3 RandomTileExceptTypeReal(HashSet<Type> except) {
		return Grid2RealPos(RandomTileExceptType(except));
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
		baseTilemap.SetColor(tilePos, new Color(1, 1, 1, 1));
	}
	
	public void ClearOverlayTiles() {
		overlayTilemap.ClearAllTiles();
	}
	
	public void GenerateWorld() {	
		baseTilemap.ClearAllTiles();
		int[,] mapMatrix = GenerateMapMatrix();
		
		ApplyMap(mapMatrix);
		LinkMountainRanges(mapMatrix);
		CreateLakes(mapMatrix);
		CreateRoad();
		CreateTintBuffer(mapMatrix);
		
		// how many tiles do we want shown vertically?
		int yTiles = mapDimensionY;
		CameraManager.RefitCamera(yTiles);
		
		//Vector2 minBounds = new Vector2(7, (float)yTiles/2);
		//Vector2 maxBounds = new Vector2(mapDimensionX-7, (float)yTiles/2);
		Vector2 minBounds = new Vector2(7, (float)yTiles/2);
		Vector2 maxBounds = new Vector2(mapDimensionX-7, (float)yTiles/2);
		CameraManager.SetBounds(minBounds, maxBounds);
    }
	
	private int[,] GenerateMapMatrix() {
		int[,] mapMatrix = new int[mapDimensionX, mapDimensionY];
		
		// randomly select all other tiles
		for (int i = 0; i < mapMatrix.GetLength(0); i++) {
			for (int j = 0; j < mapMatrix.GetLength(1); j++) {
				// this determines which tile is chosen
				var rng = Random.Range(1, 101); // exclusive
				
				int selection;
				int probCounter = 0;
				for(selection = 0; selection < tileOptions.Count; selection++) {
					probCounter += tileOptions[selection].probability;
					if (rng <= probCounter) break;
				}
				Debug.Assert(probCounter <= 100);
				mapMatrix[i, j] = selection;
			}
		}
		
		return mapMatrix;
	}
	
	private void ApplyMap(int[,] mapMatrix) {
		var currentPos = baseTilemap.origin;

		for (int x = 0; x < mapMatrix.GetLength(0); x++) {
			for (int y = 0; y < mapMatrix.GetLength(1); y++) {
				
				var tileChoice = tileOptions[mapMatrix[x, y]];
				SetAppropriateTile(currentPos, tileChoice);
				
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
		
	private void LinkMountainRanges(int[,] mapMatrix) {
		// choose some %age of mountains to link
		// "3" is mountain type in mapMatrix
		List<Vector3Int> posList = PositionsOfType(mapMatrix, 3);
		List<Vector3Int> toLink = Utils.RandomSelections<Vector3Int>(posList, (int)(posList.Count/2));
		
		// for each linking mountain, find the closest next mountain, and link to it
		foreach(Vector3Int startMountain in toLink) {
			Vector3Int endMountain = ClosestOfType(startMountain, mapMatrix, 3);
			
			// create paths between them
			// for each path, replace the tile with a mountain tile
			MovingObjectPath mRange = MovingObjectPath.GetAnyPathTo(startMountain, endMountain);
			Vector3Int currPos = startMountain;
			while(currPos != endMountain) {
				currPos = mRange.Next(currPos);
				SetAppropriateTile(currPos, tileOptions[3]);
			}
		}
	}
	
	private void CreateLakes(int[,] mapMatrix) {
		// choose random grass points to make into lakes
		List<Vector3Int> posList = PositionsOfType(mapMatrix, 2);	// water
		
		// TODO: this can be faster for sure
		HashSet<Vector3Int> nonMountains = new HashSet<Vector3Int>();
		foreach(Vector3Int tilePos in worldTileGrid.Keys) {
			if (worldTileGrid[tilePos].GetType() != typeof(MountainWorldTile)) {
				nonMountains.Add(tilePos);
			}
		}

		// flood fill each of these areas with randomized tile counts
		foreach(Vector3Int lakeOrigin in posList) {
			int lakeRange = Random.Range(5, 11);
			int lakeSize = Random.Range(5, 11);
			
			FlowField fField = FlowField.FlowFieldFrom(lakeOrigin, nonMountains, range: lakeRange, numElements: lakeSize);
			
			foreach(Vector3Int lakePos in fField.field.Keys) {
				SetAppropriateTile(lakePos, tileOptions[2]);
			}
		}
	}
	
	private void CreateRoad() {
		// utility func
		HashSet<Vector3Int> _HS(Vector3Int a, Vector3Int b) {
			return new HashSet<Vector3Int> {a, b};
		}
		
		T GetPatternTile<T>(HashSet<Vector3Int> pattern) where T : WorldTile {
			MethodInfo methodInfo = typeof(T).GetMethod("GetTileWithSprite");
			
			Dictionary<HashSet<Vector3Int>, T> patternToTile = new Dictionary<HashSet<Vector3Int>, T>(HashSet<Vector3Int>.CreateSetComparer()) {
				[_HS(Vector3Int.left, Vector3Int.right)] = (T)methodInfo.Invoke(null, new object[] {0}),
				//			
				[_HS(Vector3Int.up, Vector3Int.left)]	 = (T)methodInfo.Invoke(null, new object[] {1}),
				[_HS(Vector3Int.up, Vector3Int.right)]	 = (T)methodInfo.Invoke(null, new object[] {2}),
				//
				[_HS(Vector3Int.down, Vector3Int.right)] = (T)methodInfo.Invoke(null, new object[] {3}),
				[_HS(Vector3Int.down, Vector3Int.left)]	 = (T)methodInfo.Invoke(null, new object[] {4}),
				//
				[_HS(Vector3Int.up, Vector3Int.down)]	 = (T)methodInfo.Invoke(null, new object[] {5})
			};
			
			return patternToTile[pattern];
		}

		Vector3Int startPos = new Vector3Int(1, Random.Range(1, mapDimensionY-1), 0);
		Vector3Int endPos   = new Vector3Int(mapDimensionX-1, Random.Range(1, mapDimensionY-1), 0);
		MovingObjectPath road = MovingObjectPath.GetAnyPathTo(startPos, endPos);
		
		// now that we have the path, place the correct road tiles
		Vector3Int prevPos = startPos;
		Vector3Int roadPos = startPos;
		Vector3Int nextPos = startPos;
		while(roadPos != endPos) {
			prevPos = roadPos;
			roadPos = nextPos;
			nextPos = road.Next(roadPos);
			
			// create a pattern, only if they're different
			if(prevPos != roadPos && roadPos != nextPos && prevPos != nextPos) {
				HashSet<Vector3Int> pattern = _HS((prevPos - roadPos), (nextPos - roadPos));
				
				Type tileType = worldTileGrid[roadPos].GetType();
				WorldTile roadTile = null;
				
				if (tileType == typeof(GrassWorldTile)) {
					roadTile = GetPatternTile<RoadWorldTile>(pattern);
				}
				else if (tileType == typeof(WaterWorldTile)) {
					roadTile = GetPatternTile<WaterRoadWorldTile>(pattern);
				}
				else if (tileType == typeof(MountainWorldTile)) {
					roadTile = GetPatternTile<MountainRoadWorldTile>(pattern);
				}
				SetAppropriateTile(roadPos, roadTile);
			}
		}
	}
	
	private void CreateTileBuffer(int[,] mapMatrix) {
		// now that we have a world, create an out-of-bounds region for display purposes
		List<CloudWorldTile> bufferTiles = new List<CloudWorldTile>{
			CloudWorldTile.GetTileWithSprite(0),
			CloudWorldTile.GetTileWithSprite(1),
			CloudWorldTile.GetTileWithSprite(2)
		};
		int buffer = bufferTiles.Count+3;
		
		int xUpper = mapMatrix.GetLength(0);
		int yUpper = mapMatrix.GetLength(1);
		for (int x = -buffer; x < xUpper+buffer; x++) {			
			for (int y = -buffer; y < yUpper+buffer; y++) {
				if ((x > -1 && y > -1) && (x < xUpper && y < yUpper)) continue;
				
				CloudWorldTile tileChoice = bufferTiles[2];
				if ((x >= -1 && x <= xUpper+1) && (y >= -1 && y <= yUpper+1)) {
					tileChoice = bufferTiles[0];
				} else if ((x >= -2 && x <= xUpper+2) && (y >= -2 && y <= yUpper+2)) {
					tileChoice = bufferTiles[1];
				}
				
				// set the WorldTile in the actual tilemap
				baseTilemap.SetTile(new Vector3Int(x, y, 0), tileChoice);
			}
		}
	}
	
	private void CreateTintBuffer(int[,] mapMatrix) {
		int buffer = 5;
		
		int xUpper = mapMatrix.GetLength(0);
		int yUpper = mapMatrix.GetLength(1);
		for (int x = -buffer; x < xUpper+buffer; x++) {			
			for (int y = -buffer; y < yUpper+buffer; y++) {
				if ((x > -1 && y > -1) && (x < xUpper && y < yUpper)) continue;
				
				// set the WorldTile in the actual tilemap
				Vector3Int tilePos = new Vector3Int(x, y, 0);
				depthTilemap.SetTile(tilePos, ScriptableObject.CreateInstance<MountainWorldTile>() as MountainWorldTile);
				
				if ((x >= -1 && x <= xUpper+1) && (y >= -1 && y <= yUpper+1)) {
					TintTile(tilePos, new Color(.85f, .85f, .85f));
				} else if ((x >= -2 && x <= xUpper+2) && (y >= -2 && y <= yUpper+2)) {
					TintTile(tilePos, new Color(.55f, .55f, .55f));
				} else {
					TintTile(tilePos, new Color(.3f, .3f, .3f));
				}
			}
		}
	}
	
	private List<Vector3Int> PositionsOfType(int[,] mapMatrix, int type) {
		List<Vector3Int> positionList = new List<Vector3Int>();
		
		for (int x = 0; x < mapMatrix.GetLength(0); x++) {
			for (int y = 0; y < mapMatrix.GetLength(1); y++) {
				if (mapMatrix[x, y] == type) {
					positionList.Add(new Vector3Int(x, y, 0));
				}
			}
		}
		return positionList;
	}
	
	private Vector3Int ClosestOfType(Vector3Int startPos, int[,] mapMatrix, int type) {
		Vector3Int retVal = startPos;
		float currDist = (float)(mapMatrix.GetLength(0) + 1);
		
		for (int x = 0; x < mapMatrix.GetLength(0); x++) {
			for (int y = 0; y < mapMatrix.GetLength(1); y++) {
				if (mapMatrix[x, y] == type) {
					Vector3Int currPos = new Vector3Int(x, y, 0);
					if (currPos == startPos) continue;
					
					float dist = Vector3Int.Distance(startPos, currPos);
					if (dist < currDist) {
						currDist = dist;
						retVal = currPos;
					}
				}
			}
		}
		return retVal;
	}
}
