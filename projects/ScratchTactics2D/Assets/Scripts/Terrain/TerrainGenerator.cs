using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

public abstract class TerrainGenerator : MonoBehaviour
{
	public enum TileEnum {none, grass, sand, forest, water, deepWater, foothills, mountain, peak, mountain2x2, village, ruins, x};

	public Vector2Int mapDimension;
	protected int mapDimensionX { get => mapDimension.x; }
	protected int mapDimensionY { get => mapDimension.y; }
	protected TileEnum[,] map;

	protected static WorldTile[] tileOptions;
	public Action<Vector3Int, WorldTile> TileSetter;
	
	public abstract void GenerateMap();
	// virtual: Preprocessing();
	// virtual: ApplyMap();
	// virtual: Postprocessing();

	void Awake() {
		tileOptions = new WorldTile[]{
			(ScriptableObject.CreateInstance<XWorldTile>() as XWorldTile),
			(ScriptableObject.CreateInstance<GrassWorldTile>() as GrassWorldTile),
			(ScriptableObject.CreateInstance<SandWorldTile>() as SandWorldTile),
			(ScriptableObject.CreateInstance<ForestWorldTile>() as ForestWorldTile),
			(ScriptableObject.CreateInstance<WaterWorldTile>() as WaterWorldTile),
			(ScriptableObject.CreateInstance<DeepWaterWorldTile>() as DeepWaterWorldTile),
			(ScriptableObject.CreateInstance<FoothillsWorldTile>() as FoothillsWorldTile),
			(ScriptableObject.CreateInstance<MountainWorldTile>() as MountainWorldTile),
			(ScriptableObject.CreateInstance<PeakWorldTile>() as PeakWorldTile),
			(ScriptableObject.CreateInstance<Mountain2x2WorldTile>() as Mountain2x2WorldTile),
			(ScriptableObject.CreateInstance<VillageWorldTile>() as VillageWorldTile),
			(ScriptableObject.CreateInstance<RuinsWorldTile>() as RuinsWorldTile),
			(ScriptableObject.CreateInstance<XWorldTile>() as XWorldTile)
		};
	}
	
	public void SetTileSetter(Action<Vector3Int, WorldTile> tileSetter) {
		TileSetter = tileSetter;
	}

	public static WorldTile TileOption(TileEnum tileType) {
		return tileOptions[(int)tileType];
	}

	public TileEnum[,] GetMap() {
		return map;
	}

	public virtual void ApplyMap(Tilemap tilemap) {
		var currentPos = tilemap.origin;

		for (int x = 0; x < map.GetLength(0); x++) {
			for (int y = 0; y < map.GetLength(1); y++) {
				
				var tileChoice = TileOption(map[x, y]);
				TileSetter(currentPos, tileChoice);
				
				currentPos = new Vector3Int(currentPos.x,
											(int)(currentPos.y+tilemap.cellSize.y),
											currentPos.z);
			}
			currentPos = new Vector3Int((int)(currentPos.x+tilemap.cellSize.x),
										tilemap.origin.y,
										currentPos.z);
		}
		//
		tilemap.CompressBounds();
		tilemap.RefreshAllTiles();
	}

    public virtual void Preprocessing() {
		Debug.Log($"Preprocessing: mountain2x2 replacement");

        // replace 2x2 mountains with large mountain tiles
		// create bottom-left 2x2 pattern for each mountain
		//foreach (var mntPos in PositionsOfType(TileEnum.mountain)) {
		// if (mntPos.x >= map.GetLength(0)-2 || mntPos.y >= map.GetLength(1)-2) continue;

		for (int x = map.GetLength(0)-2; x >= 0 ; x--) {
			for (int y = map.GetLength(1)-2; y >= 0; y--) {

				// check here, to modify while iterating
				if (map[x, y] == TileEnum.mountain || map[x, y] == TileEnum.peak) {
					var mntPos = new Vector3Int(x, y, 0);
				
					List<Vector3Int> _pattern = new List<Vector3Int>{
						mntPos,
						mntPos + Vector3Int.right,
						mntPos + Vector3Int.up,
						mntPos + Vector3Int.right + Vector3Int.up,
					}.Where( it => map[it.x, it.y] == TileEnum.mountain || map[it.x, it.y] == TileEnum.peak).ToList();
					TerrainPatternShape pattern = TerrainPatternShape.FromList( _pattern.Select(it => it - mntPos).ToList() );

					// if the filtered pattern matches a square:
					if (TerrainPatternShape.BottomLeftSquare.Matches(pattern)) {
						Debug.Log($"{mntPos} passed!");
						_pattern.ForEach( it => Debug.Log($"> {it} ({map[it.x, it.y]})") );

						map[mntPos.x,   mntPos.y]   = TileEnum.mountain2x2;
						map[mntPos.x+1, mntPos.y]   = TileEnum.none;
						map[mntPos.x,   mntPos.y+1] = TileEnum.none;
						map[mntPos.x+1, mntPos.y+1] = TileEnum.none;
					}
				}
			}
		}
    }

	public virtual void Postprocessing() {}

	protected List<Vector3Int> PositionsOfType(TileEnum type) {
		List<Vector3Int> positionList = new List<Vector3Int>();
		
		for (int x = 0; x < map.GetLength(0); x++) {
			for (int y = 0; y < map.GetLength(1); y++) {
				if (map[x, y] == type) {
					positionList.Add(new Vector3Int(x, y, 0));
				}
			}
		}
		return positionList;
	}
	
	protected Vector3Int ClosestOfType(Vector3Int startPos, TileEnum type) {
		Vector3Int retVal = startPos;
		float currDist = (float)(map.GetLength(0) + 1);
		
		for (int x = 0; x < map.GetLength(0); x++) {
			for (int y = 0; y < map.GetLength(1); y++) {
				if (map[x, y] == type) {
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