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
	private Dictionary<Vector3Int, Terrain> terrain;
	public IEnumerable<Terrain> Terrain { get => terrain.Values; }
	public IEnumerable<Vector3Int> Positions { get => terrain.Keys; }

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

    // IPathable definitions
    public override int EdgeCost(Vector3Int src, Vector3Int dest) {
		return TerrainAt(dest).tickCost;
    }

	public override bool IsInBounds(Vector3Int tilePos) {
		return terrain.ContainsKey(tilePos);
	}
		
	public override Tile GetTileAt(Vector3Int tilePos) {
		return baseTilemap.GetTile(tilePos) as Tile;
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
		TintTile(baseTilemap, tilePos, color);
	}

	public void ResetHighlightTile(Vector3Int tilePos) {
		ResetTintTile(baseTilemap, tilePos);
	}
	
	public void ResetHighlightTiles(HashSet<Vector3Int> tilePosSet) {
		foreach (var tilePos in tilePosSet) {
			ResetTintTile(baseTilemap, tilePos);
		}
	}

	public void ResetAllHighlightTiles() {
		foreach (var tilePos in terrain.Keys) {
			ResetTintTile(baseTilemap, tilePos);
		}
	}
	
	public override void TintTile(Tilemap tilemap, Vector3Int tilePos, Color color) {
		if (tilemap.GetTile(tilePos) != null) {
			tilemap.SetTileFlags(tilePos, TileFlags.None);
			tilemap.SetColor(tilePos, color);
			return;
		} else {
			Debug.Log("Not a valid Tint target");
			Debug.Assert(false);
		}
	}
	
	public override void ResetTintTile(Tilemap tilemap, Vector3Int tilePos) {
		tilemap.SetTileFlags(tilePos, TileFlags.None);
		tilemap.SetColor(tilePos, new Color(1, 1, 1, 1));
	}

	public void HighlightTileTween(Vector3Int tilePos, Color color) {
		if (baseTilemap.GetColor(tilePos) != color) {
			StartCoroutine( TintTileTween(tilePos, color) );
		}
	}

	private IEnumerator TintTileTween(Vector3Int tilePos, Color color) {
		Color initColor = baseTilemap.GetColor(tilePos);

		float c = 0.0f;
		while (c < 1.0f) {
			HighlightTile(tilePos, Color.Lerp(initColor, color, c));
			c += 0.05f;
			yield return null;
		}
	}
	
	public void ClearOverlayTiles() {
		overlayTilemap.ClearAllTiles();
	}

	// given a FOV, update all entities' visibility states
	public void UpdateVisibility(FieldOfView fov) {
		foreach (EnemyArmy army in FindObjectsOfType<EnemyArmy>().OfType<IVisible>()) {
			if (fov.field.ContainsKey(army.gridPosition)) {
				army.visible = fov.field[army.gridPosition];
			} else {
				army.visible = Enum.VisibleState.hidden;
			}
		}
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
		
		Vector2 minBounds = new Vector2(4, 2.5f);
		Vector2 maxBounds = new Vector2(tg.mapDimension.x-4, (float)tg.mapDimension.y - 2.5f);
		Camera.main.GetComponent<CameraManager>().SetBounds(minBounds, maxBounds);
		Camera.main.GetComponent<CameraManager>().RefitCamera(zoomLevel);
    }

	private void CreateTintBuffer(TerrainGenerator.WorldTileEnum[,] map) {
		int buffer = 5;
		
		int xUpper = map.GetLength(0);
		int yUpper = map.GetLength(1);
		for (int x = -buffer; x < xUpper+buffer; x++) {			
			for (int y = -buffer; y < yUpper+buffer; y++) {
				if ((x > -1 && y > -1) && (x < xUpper && y < yUpper)) continue;
				
				// set the WorldTile in the actual tilemap
				Vector3Int tilePos = new Vector3Int(x, y, 0);
				baseTilemap.SetTile(tilePos, TerrainGenerator.TileOption(TerrainGenerator.WorldTileEnum.x));

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

	public void SetTerrainAt(Vector3Int v, Terrain t) {
		terrain[v] = t;
	}

	public Terrain TerrainAt(Vector3Int v) {
		if (terrain.ContainsKey(v)) {
			return terrain[v];
		}
		return new EmptyTerrain();
	}

	public override void UnderlayAt(Vector3Int tilePos, Color color) {
		underlayTilemap.SetTile(tilePos, selectTile);
		TintTile(underlayTilemap, tilePos, color);
	}

	public override void ResetUnderlayAt(Vector3Int tilePos) {
		underlayTilemap.SetTile(tilePos, null);
		ResetTintTile(underlayTilemap, tilePos);
	}

	// for pseudo-deprecated RandomTerrainGenerator
	public List<Vector3Int> LocationsOf<T>() where T : Terrain {
		return terrain.Keys.ToList().Where( it => terrain[it].GetType() == typeof(T)).ToList();
	}

	public Type TypeAt(Vector3Int v) {
		return terrain[v].GetType();
	}
}