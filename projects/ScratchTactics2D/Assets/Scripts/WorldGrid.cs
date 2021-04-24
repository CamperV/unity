using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

public class WorldGrid : GameGrid
{
	private enum TileEnum {grass, forest, water, deepWater, mountain, village, ruins, dirt};
	private List<WorldTile> tileOptions;
	private OverlayTile selectTile;
	private Dictionary<Vector3Int, WorldTile> worldTileGrid;
	
	private Canvas tintCanvas;
	
	protected override void Awake() {
		base.Awake();
		
		// tileOptions determine probability order as well
		// so when WorldGrid is generated, it will check if the tile is:
		//  grass first, then dirt, then water, then mountain
		tileOptions = new List<WorldTile>{
			(ScriptableObject.CreateInstance<GrassWorldTile>() as GrassWorldTile),
			(ScriptableObject.CreateInstance<ForestWorldTile>() as ForestWorldTile),
			(ScriptableObject.CreateInstance<WaterWorldTile>() as WaterWorldTile),
			(ScriptableObject.CreateInstance<DeepWaterWorldTile>() as DeepWaterWorldTile),
			(ScriptableObject.CreateInstance<MountainWorldTile>() as MountainWorldTile),
			(ScriptableObject.CreateInstance<VillageWorldTile>() as VillageWorldTile),
			(ScriptableObject.CreateInstance<RuinsWorldTile>() as RuinsWorldTile),
			(ScriptableObject.CreateInstance<DirtWorldTile>() as DirtWorldTile)
		};
		selectTile = (ScriptableObject.CreateInstance<SelectOverlayTile>() as SelectOverlayTile);
		
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
		Debug.Assert(tilePos.z == 0);

		// if anything else is there, nullify it in the map first
		// this will happen when replacing roads, etc
		var nullDepth = ((WorldTile)GetTileAt(tilePos))?.depth ?? 0;
		var nullPos = new Vector3Int(tilePos.x, tilePos.y, nullDepth);
		baseTilemap.SetTile(nullPos, null);

		worldTileGrid[tilePos] = tile;
		translation2D[new Vector2Int(tilePos.x, tilePos.y)] = tilePos;
		var depthPos = new Vector3Int(tilePos.x, tilePos.y, tile.depth);
		baseTilemap.SetTile(depthPos, tile);
	}

	// randomly permuting may actually be better than iterating?
	// nah probably not, this is lazy
	public Vector3Int RandomTileWithinType(HashSet<Type> within) {
		List<Vector3Int> pool = worldTileGrid.Keys.ToList().Where( it => within.Contains(worldTileGrid[it].GetType()) ).ToList();

		Vector3Int retVal;
		do {
			if (pool.Count == 0) {
				Debug.Log($"While trying to find a vacant tile of types {within}, all are vacant");
				return -1*Vector3Int.one;
			}
			retVal = pool.PopRandom<Vector3Int>();
		} while (!VacantAt(retVal));
		return retVal;
	}
	
	public Vector3Int RandomTileExceptType(HashSet<Type> except) {
		int x;
		int y;
		Vector3Int retVal;
		do {
			x = Random.Range(0, mapDimensionX);
			y = Random.Range(0, mapDimensionY);
			retVal = new Vector3Int(x, y, 0);
		} while (!VacantAt(retVal) || except.Contains(GetTileAt(retVal).GetType()));
		return retVal;
	}

	public void HighlightTile(Vector3Int tilePos, Color color) {
		for (int z = 0; z < 2; z++) {
			var v = new Vector3Int(tilePos.x, tilePos.y, z);
			TintTile(baseTilemap, v, color);
		}	
	}
	
	public void HighlightTiles(HashSet<Vector3Int> tilePosSet, Color color) {
		foreach (var tilePos in tilePosSet) {
			HighlightTile(tilePos, color);
		}
	}
	
	public void ResetHighlightTiles(HashSet<Vector3Int> tilePosSet) {
		foreach (var tilePos in tilePosSet) {
			for (int z = 0; z < 2; z++) {
				var v = new Vector3Int(tilePos.x, tilePos.y, z);
				ResetTintTile(baseTilemap, v);
			}
		}
	}

	public void ResetAllHighlightTiles() {
		foreach (var tilePos in worldTileGrid.Keys) {
			for (int z = 0; z < 2; z++) {
				var v = new Vector3Int(tilePos.x, tilePos.y, z);
				ResetTintTile(baseTilemap, v);
			}
		}
	}
	
	public override void TintTile(Tilemap tilemap, Vector3Int tilePos, Color color) {
		Vector3Int depthPos;
		if (worldTileGrid.ContainsKey(tilePos)) {
			var tile = worldTileGrid[tilePos];
			depthPos = new Vector3Int(tilePos.x, tilePos.y, tile.depth);
		} else  {
			depthPos = tilePos;
		}

		if (tilemap.GetTile(depthPos) != null) {
			tilemap.SetTileFlags(tilePos, TileFlags.None);
			tilemap.SetColor(tilePos, color);
			return;
		} else {
			//Debug.Log("Not a valid Tint target");
			//Debug.Assert(false);
		}
	}
	
	public override void ResetTintTile(Tilemap tilemap, Vector3Int tilePos) {
		tilemap.SetTileFlags(tilePos, TileFlags.None);
		tilemap.SetColor(tilePos, new Color(1, 1, 1, 1));
	}
	
	public void ClearOverlayTiles() {
		overlayTilemap.ClearAllTiles();
	}
	
	public void GenerateWorld() {	
		baseTilemap.ClearAllTiles();
		int[,] mapMatrix = GenerateMapMatrix();

		ApplyMap(mapMatrix);
		LinkMountainRanges(mapMatrix);
		CreateForests(mapMatrix, 2, 5);
		CreateLakes(mapMatrix, 5, 10);

		// player-affecting tiles
		PlaceVillages(mapMatrix, 10);
		CreateRoadBetweenWaypoints( LocationsOf<VillageWorldTile>() );
		

		// finalize outside
		CreateTintBuffer(mapMatrix);
		
		// how many tiles do we want shown vertically?
		CameraManager.RefitCamera(5);
		
		Vector2 minBounds = new Vector2(4, 2.5f);
		Vector2 maxBounds = new Vector2(mapDimensionX-4, (float)mapDimensionY - 2.5f);
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
		List<Vector3Int> posList = PositionsOfType(mapMatrix, TileEnum.mountain);
		List<Vector3Int> toLink = posList.RandomSelections<Vector3Int>((int)(posList.Count/2));
		
		// for each linking mountain, find the closest next mountain, and link to it
		foreach(Vector3Int startMountain in toLink) {
			Vector3Int endMountain = ClosestOfType(startMountain, mapMatrix, TileEnum.mountain);
			
			// create paths between them
			// for each path, replace the tile with a mountain tile
			MovingObjectPath mRange = MovingObjectPath.GetAnyPathTo(startMountain, endMountain);
			Vector3Int currPos = startMountain;
			while(currPos != endMountain) {
				currPos = mRange.Next(currPos);
				SetAppropriateTile(currPos, tileOptions[(int)TileEnum.mountain]);
			}
		}
	}

	private void CreateForests(int[,] mapMatrix, int lowerBound = 4, int upperBound = 9) {
		// choose random grass points to make into lakes
		List<Vector3Int> posList = PositionsOfType(mapMatrix, TileEnum.forest);
		
		// TODO: this can be faster for sure
		HashSet<Vector3Int> nonMountains = new HashSet<Vector3Int>();
		foreach(Vector3Int tilePos in worldTileGrid.Keys) {
			if (worldTileGrid[tilePos].GetType() != typeof(MountainWorldTile)) {
				nonMountains.Add(tilePos);
			}
		}

		// flood fill each of these areas with randomized tile counts
		foreach(Vector3Int lakeOrigin in posList) {
			int lakeRange = Random.Range(lowerBound, upperBound);
			int lakeSize = Random.Range(lowerBound, upperBound);
			
			FlowField fField = FlowField.FlowFieldFrom(lakeOrigin, nonMountains, range: lakeRange*Constants.standardTickCost, numElements: lakeSize);
			
			foreach(Vector3Int lakePos in fField.field.Keys) {
				SetAppropriateTile(lakePos, tileOptions[(int)TileEnum.forest]);
			}
		}
	}
	
	private void CreateLakes(int[,] mapMatrix, int lowerBound = 4, int upperBound = 9) {
		// choose random grass points to make into lakes
		List<Vector3Int> posList = PositionsOfType(mapMatrix, TileEnum.water);
		
		// TODO: this can be faster for sure
		HashSet<Vector3Int> nonMountains = new HashSet<Vector3Int>();
		foreach(Vector3Int tilePos in worldTileGrid.Keys) {
			if (worldTileGrid[tilePos].GetType() != typeof(MountainWorldTile)) {
				nonMountains.Add(tilePos);
			}
		}

		// flood fill each of these areas with randomized tile counts
		foreach(Vector3Int lakeOrigin in posList) {
			int lakeRange = Random.Range(lowerBound, upperBound);
			int lakeSize = Random.Range(lowerBound, upperBound);
			
			FlowField fField = FlowField.FlowFieldFrom(lakeOrigin, nonMountains, range: lakeRange*Constants.standardTickCost, numElements: lakeSize);
			foreach(Vector3Int lakePos in fField.field.Keys) {
				SetAppropriateTile(lakePos, tileOptions[(int)TileEnum.water]);
			}
		}

		// now that the tiles are placed, check for deep water
		foreach(Vector3Int tilePos in LocationsOf<WaterWorldTile>()) {
			bool surrounded = true;
			foreach (var neighbor in GetEightNeighbors(tilePos)) {
				surrounded &= (worldTileGrid[neighbor].GetType() != typeof(GrassWorldTile) &&
							   worldTileGrid[neighbor].GetType() != typeof(ForestWorldTile));
			}
			if (surrounded) SetAppropriateTile(tilePos, tileOptions[(int)TileEnum.deepWater]);
		}
	}

	// wow I was very lazy when I implemented this
	// just keep it man, there's too much to do
	private void PlaceVillages(int[,] mapMatrix, int num) {
		if (num < 2) {
			Debug.Log("Need to place at least a starting and ending village");
			return;
		}

		// first village
		// the rest are randomized
		Vector3Int firstVillagePos = new Vector3Int(1, Random.Range(1, mapDimensionY-1), 0);
		SetAppropriateTile(firstVillagePos, tileOptions[(int)TileEnum.village]);

		Vector3Int lastVillagePos  = new Vector3Int(mapDimensionX-1, Random.Range(1, mapDimensionY-1), 0);
		SetAppropriateTile(lastVillagePos, tileOptions[(int)TileEnum.village]);

		foreach (Vector3Int villagePos in PositionsOfType(mapMatrix, TileEnum.grass).RandomSelections<Vector3Int>((int)(num-2))) {
			SetAppropriateTile(villagePos, tileOptions[(int)TileEnum.village]);
		}
	}

	private void CreateRoadBetweenWaypoints(List<Vector3Int> waypoints) {
		Vector3Int prevPos = Vector3Int.zero;
		int i = 0;

		foreach (Vector3Int pos in waypoints.OrderBy(it => it.x)) {
			if (i > 0) {
				new Road(prevPos, pos).Apply(this);
			}
			prevPos = pos;
			i++;
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
				baseTilemap.SetTile(new Vector3Int(x, y, tileChoice.depth), tileChoice);
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
				Vector3Int tilePos = new Vector3Int(x, y, tileOptions[(int)TileEnum.mountain].depth);
				baseTilemap.SetTile(tilePos, tileOptions[(int)TileEnum.mountain]);

				if ((x >= -1 && x <= xUpper+1) && (y >= -1 && y <= yUpper+1)) {
					TintTile(baseTilemap, tilePos, new Color(0.80f, 0.80f, 0.80f));
				} else if ((x >= -2 && x <= xUpper+2) && (y >= -2 && y <= yUpper+2)) {
					TintTile(baseTilemap, tilePos, new Color(0.50f, 0.50f, 0.50f));
				} else {
					TintTile(baseTilemap, tilePos, new Color(0.3f, 0.3f, 0.3f));
				}
			}
		}
	}
	
	private List<Vector3Int> PositionsOfType(int[,] mapMatrix, TileEnum type) {
		List<Vector3Int> positionList = new List<Vector3Int>();
		
		for (int x = 0; x < mapMatrix.GetLength(0); x++) {
			for (int y = 0; y < mapMatrix.GetLength(1); y++) {
				if (mapMatrix[x, y] == (int)type) {
					positionList.Add(new Vector3Int(x, y, 0));
				}
			}
		}
		return positionList;
	}
	
	private Vector3Int ClosestOfType(Vector3Int startPos, int[,] mapMatrix, TileEnum type) {
		Vector3Int retVal = startPos;
		float currDist = (float)(mapMatrix.GetLength(0) + 1);
		
		for (int x = 0; x < mapMatrix.GetLength(0); x++) {
			for (int y = 0; y < mapMatrix.GetLength(1); y++) {
				if (mapMatrix[x, y] == (int)type) {
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

	private List<Vector3Int> LocationsOf<T>() where T : WorldTile {
		return worldTileGrid.Keys.ToList().Where( it => worldTileGrid[it].GetType() == typeof(T)).ToList();
	}

	public Type TypeAt(Vector3Int v) {
		return worldTileGrid[v].GetType();
	}
}
