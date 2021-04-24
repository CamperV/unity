using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

public class Road : WorldTerrain
{
	public Vector3Int start;
	public Vector3Int end;

	private MovingObjectPath path;

	public Road(Vector3Int startPos, Vector3Int endPos) {
		start = startPos;
		end = endPos;
		path = MovingObjectPath.GetAnyPathTo(startPos, endPos);
	}

	public override void Apply(WorldGrid grid) {
		// TODO - change this to not rely on the road, but rather check all eight neighbors for each road tile
		Vector3Int prevPos = start;
		Vector3Int roadPos = start;
		Vector3Int nextPos = start;

		while(roadPos != end) {
			prevPos = roadPos;
			roadPos = nextPos;
			nextPos = path.Next(roadPos);
			
			// create a pattern, only if they're different
			if(prevPos != roadPos && roadPos != nextPos && prevPos != nextPos) {
				HashSet<Vector3Int> pattern = _HS((prevPos - roadPos), (nextPos - roadPos));
				
				Type tileType = grid.TypeAt(roadPos);
				WorldTile roadTile = null;
				
				if (tileType == typeof(GrassWorldTile)) {
					roadTile = GetPatternTile<RoadWorldTile>(pattern);
				}
				else if (tileType == typeof(ForestWorldTile)) {
					roadTile = GetPatternTile<ForestRoadWorldTile>(pattern);
				}				
				else if (tileType == typeof(WaterWorldTile)) {
					roadTile = GetPatternTile<WaterRoadWorldTile>(pattern);
				}
				else if (tileType == typeof(DeepWaterWorldTile)) {
					roadTile = GetPatternTile<WaterRoadWorldTile>(pattern);
				}
				else if (tileType == typeof(MountainWorldTile)) {
					roadTile = GetPatternTile<MountainRoadWorldTile>(pattern);
				} else {
					roadTile = GetPatternTile<RoadWorldTile>(pattern);
				}

				grid.SetAppropriateTile(roadPos, roadTile);
			}
		}
	}

	private T GetPatternTile<T>(HashSet<Vector3Int> pattern) where T : WorldTile {
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

	// utility func
	HashSet<Vector3Int> _HS(Vector3Int a, Vector3Int b) {
		return new HashSet<Vector3Int> {a, b};
	}
}