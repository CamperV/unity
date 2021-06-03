using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

public class Overworld : GameGrid, IPathable
{	
	public int zoomLevel;	// in vertical tiles
	public TerrainGenerator terrainGenerator;	// assigned prefab
	public Dictionary<Vector3Int, Terrain> terrain;

	private OverlayTile selectTile;
	
	private Canvas tintCanvas;
	
	protected override void Awake() {
		base.Awake();

		selectTile = (ScriptableObject.CreateInstance<SelectOverlayTile>() as SelectOverlayTile);
		//
		terrain = new Dictionary<Vector3Int, Terrain>();
		
		// set camera and canvas
		tintCanvas = GetComponentsInChildren<Canvas>()[0];
		tintCanvas.renderMode = RenderMode.ScreenSpaceCamera;
		tintCanvas.worldCamera = Camera.main;
		tintCanvas.sortingLayerName = "Overworld Entities";
		tintCanvas.sortingOrder = 1;
	}

	public override bool IsInBounds(Vector3Int tilePos) {
		return terrain.ContainsKey(tilePos);
	}
		
	public override GameTile GetTileAt(Vector3Int tilePos) {
		return baseTilemap.GetTile(tilePos) as GameTile;
	}

	public override HashSet<Vector3Int> GetAllTilePos() {
		return new HashSet<Vector3Int>(terrain.Keys);
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
		translation2D[new Vector2Int(tilePos.x, tilePos.y)] = tilePos;
		baseTilemap.SetTile(tilePos, tile);
	}

	// randomly permuting may actually be better than iterating?
	// nah probably not, this is lazy
	public Vector3Int RandomTileWithinType(HashSet<Type> within) {
		List<Vector3Int> pool = terrain.Keys.ToList().Where( it => within.Contains(terrain[it].GetType()) ).ToList();

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
		int upperX = terrain.Keys.Select(it => it.x).Max();
		int upperY = terrain.Keys.Select(it => it.y).Max();
		int x;
		int y;
		Vector3Int retVal;
		do {
			x = Random.Range(0, upperX);
			y = Random.Range(0, upperY);
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
		foreach (var tilePos in terrain.Keys) {
			for (int z = 0; z < 2; z++) {
				var v = new Vector3Int(tilePos.x, tilePos.y, z);
				ResetTintTile(baseTilemap, v);
			}
		}
	}
	
	public override void TintTile(Tilemap tilemap, Vector3Int tilePos, Color color) {
		// Vector3Int depthPos;
		// if (terrain.ContainsKey(tilePos)) {
		// 	var tile = terrain[tilePos].tile;
		// 	depthPos = new Vector3Int(tilePos.x, tilePos.y, tile.depth);
		// } else  {
		// 	depthPos = tilePos;
		// }

		if (tilemap.GetTile(tilePos) != null) {
			tilemap.SetTileFlags(tilePos, TileFlags.None);
			tilemap.SetColor(tilePos, color);
			return;
		} else {
			// Debug.Log("Not a valid Tint target");
			// Debug.Assert(false);
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

		TerrainGenerator tg = Instantiate(terrainGenerator);
		tg.GenerateMap();
		tg.SetTileSetter(SetAppropriateTile);
		tg.ApplyMap(baseTilemap);
		terrain = tg.GetTerrain();

		// finalize outside
		CreateTintBuffer( tg.GetMap() );
		
		// how many tiles do we want shown vertically?
		CameraManager.RefitCamera(zoomLevel);
		//CameraManager.RefitStaticCamera(new Vector3(tg.mapDimension.x/2f, tg.mapDimension.y/2f, -10f), tg.mapDimension.y+8);
		
		Vector2 minBounds = new Vector2(4, 2.5f);
		Vector2 maxBounds = new Vector2(tg.mapDimension.x-4, (float)tg.mapDimension.y - 2.5f);
		CameraManager.SetBounds(minBounds, maxBounds);
    }

	private void CreateTintBuffer(TerrainGenerator.TileEnum[,] map) {
		int buffer = 5;
		
		int xUpper = map.GetLength(0);
		int yUpper = map.GetLength(1);
		for (int x = -buffer; x < xUpper+buffer; x++) {			
			for (int y = -buffer; y < yUpper+buffer; y++) {
				if ((x > -1 && y > -1) && (x < xUpper && y < yUpper)) continue;
				
				// set the WorldTile in the actual tilemap
				Vector3Int tilePos = new Vector3Int(x, y, TerrainGenerator.TileOption(TerrainGenerator.TileEnum.x).depth);
				baseTilemap.SetTile(tilePos, TerrainGenerator.TileOption(TerrainGenerator.TileEnum.x));

				if ((x >= -1 && x <= xUpper+1) && (y >= -1 && y <= yUpper+1)) {
					TintTile(baseTilemap, tilePos, new Color(0.90f, 0.90f, 0.90f));
				} else if ((x >= -2 && x <= xUpper+2) && (y >= -2 && y <= yUpper+2)) {
					TintTile(baseTilemap, tilePos, new Color(0.70f, 0.70f, 0.70f));
				} else {
					TintTile(baseTilemap, tilePos, new Color(0.40f, 0.40f, 0.40f));
				}
			}
		}
	}

	public List<Vector3Int> LocationsOf<T>() where T : Terrain {
		return terrain.Keys.ToList().Where( it => terrain[it].GetType() == typeof(T)).ToList();
	}

	public List<Vector3Int> LocationsExceptOf<T>() where T : Terrain {
		return terrain.Keys.ToList().Where( it => terrain[it].GetType() != typeof(T)).ToList();
	}

	public Type TypeAt(Vector3Int v) {
		return terrain[v].GetType();
	}

	public void SetTerrainAt(Vector3Int v, Terrain t) {
		terrain[v] = t;
	}

	public Terrain TerrainAt(Vector3Int v) {
		if (terrain.ContainsKey(v)) {
			return terrain[v];
		}
		return new EmptyTerrain();
	}

    // IPathable definitions
    public IEnumerable<Vector3Int> GetNeighbors(Vector3Int origin) {
        Vector3Int up = origin + Vector3Int.up;
        Vector3Int right = origin + Vector3Int.right;
        Vector3Int down = origin + Vector3Int.down;
        Vector3Int left = origin + Vector3Int.left;
        if (terrain.ContainsKey(up)) yield return up;
        if (terrain.ContainsKey(right)) yield return right;
        if (terrain.ContainsKey(down)) yield return down;
        if (terrain.ContainsKey(left)) yield return left;
    }

    public int EdgeCost(Vector3Int src, Vector3Int dest) {
        return 1;
    }

	public override void UnderlayAt(Vector3Int tilePos, Color color) {
		underlayTilemap.SetTile(tilePos, selectTile);
		TintTile(underlayTilemap, tilePos, color);
	}

	public override void ResetUnderlayAt(Vector3Int tilePos) {
		underlayTilemap.SetTile(tilePos, null);
		ResetTintTile(underlayTilemap, tilePos);
	}

	public void HideAt(Vector3Int tilePos, float intensity = 0.15f) {
		HighlightTile(tilePos, (intensity*Color.white).WithAlpha(1.0f));
	}
}