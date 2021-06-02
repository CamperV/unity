using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;
using MapTools;

public abstract class TerrainGenerator : MonoBehaviour
{
	// NOTE:
	// this does not contain an exhaustive list of tiles. ex: road tiles
	public enum TileEnum {
		none,
		deepWater, water,
		sand,
		grass,
		forest, deepForest,
		foothills, mountain, mountain2x2, peak, peak2x2,
		village,
		ruins,
		x
	};

	public Vector2Int mapDimension;
	protected int mapDimensionX { get => mapDimension.x; }
	protected int mapDimensionY { get => mapDimension.y; }
	protected TileEnum[,] map;

	// generation parameters
	public int numVillages;

	protected static WorldTile[] tileOptions;
	public Action<Vector3Int, WorldTile> TileSetter;
	
	public abstract void GenerateMap();
	// virtual: Preprocessing();
	// virtual: Postprocessing();

	void Awake() {
		tileOptions = new WorldTile[]{
			(ScriptableObject.CreateInstance<XWorldTile>() as XWorldTile),
			//
			(ScriptableObject.CreateInstance<DeepWaterWorldTile>() as DeepWaterWorldTile),
			(ScriptableObject.CreateInstance<WaterWorldTile>() as WaterWorldTile),
			//
			(ScriptableObject.CreateInstance<SandWorldTile>() as SandWorldTile),
			//
			(ScriptableObject.CreateInstance<GrassWorldTile>() as GrassWorldTile),
			//
			(ScriptableObject.CreateInstance<ForestWorldTile>() as ForestWorldTile),
			(ScriptableObject.CreateInstance<DeepForestWorldTile>() as DeepForestWorldTile),
			//
			(ScriptableObject.CreateInstance<FoothillsWorldTile>() as FoothillsWorldTile),
			(ScriptableObject.CreateInstance<MountainWorldTile>() as MountainWorldTile),
			(ScriptableObject.CreateInstance<Mountain2x2WorldTile>() as Mountain2x2WorldTile),
			(ScriptableObject.CreateInstance<PeakWorldTile>() as PeakWorldTile),
			(ScriptableObject.CreateInstance<Peak2x2WorldTile>() as Peak2x2WorldTile),
			//
			(ScriptableObject.CreateInstance<VillageWorldTile>() as VillageWorldTile),
			//
			(ScriptableObject.CreateInstance<RuinsWorldTile>() as RuinsWorldTile),
			//
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

	public void ApplyMap(Tilemap tilemap) {
		Preprocessing();
		//

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

		//
		Postprocessing();
	}

    protected virtual void Preprocessing() {
        // replace 2x2 mountains with large mountain tiles
		// create bottom-left 2x2 pattern for each mountain
		PatternReplaceMultiple(TerrainPatternShape.BottomLeftSquare, TileEnum.peak, TileEnum.peak2x2, TileEnum.peak);
		PatternReplaceMultiple(TerrainPatternShape.BottomLeftSquare, TileEnum.mountain, TileEnum.mountain2x2, TileEnum.mountain);
    }
	protected virtual void Postprocessing() {}
	
	protected void PatternReplaceMultiple(TerrainPattern pattern, TileEnum toReplace, TileEnum replaceWith, params TileEnum[] patternContent) {
		for (int x = map.GetLength(0)-1; x >= 0 ; x--) {
			for (int y = map.GetLength(1)-1; y >= 0; y--) {
				if (map[x, y] == toReplace) {
					Vector3Int origin = new Vector3Int(x, y, 0);

					// if all pos caught in pattern match any of the patternContent:
					bool matchAllPositions = true;
					foreach (var v in pattern.YieldPattern(origin)) {
						if (map.Contains(v.x, v.y)) {
							bool matchAnyTypes = false;
							foreach(var patternMatcher in patternContent) {
								matchAnyTypes |= map[v.x, v.y] == patternMatcher;
							}
							matchAllPositions &= matchAnyTypes;
						}
					}

					if (matchAllPositions) {
						map[origin.x, origin.y] = replaceWith;

						// in the case of mountains, we need to make sure other mountain tiles aren't placed here
						// but sometimes we only want to modifiy the origin
						foreach (var v in pattern.YieldPatternExcept(origin, origin)) {

							if (map.Contains(v.x, v.y)) {
								map[v.x, v.y] = TileEnum.none;
							}
						}
					}
				}
			}
		}
	}

	protected void PatternReplaceSingle(TerrainPattern pattern, TileEnum toReplace, TileEnum replaceWith, params TileEnum[] patternContent) {
		List<Vector3Int> secondPass = new List<Vector3Int>();

		for (int x = map.GetLength(0)-1; x >= 0 ; x--) {
			for (int y = map.GetLength(1)-1; y >= 0; y--) {
				if (map[x, y] == toReplace) {
					Vector3Int origin = new Vector3Int(x, y, 0);

					// if all pos caught in pattern match any of the patternContent:
					bool matchAllPositions = true;
					foreach (var v in pattern.YieldPattern(origin)) {
						// check bounds here for the first time
						if (map.Contains(v.x, v.y)) {
							bool matchAnyTypes = false;
							foreach(var patternMatcher in patternContent) {
								matchAnyTypes |= map[v.x, v.y] == patternMatcher;
							}
							matchAllPositions &= matchAnyTypes;
						}
					}

					if (matchAllPositions) secondPass.Add(origin);
				}
			}
		}

		// perform all replacements in a second pass so that they do not affect one another
		foreach (var og in secondPass) {
			map[og.x, og.y] = replaceWith;
		}
	}
}