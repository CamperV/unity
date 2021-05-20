using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

public class River : Terrain
{
	public override void Apply(WorldGrid grid) {}

	public static HashSet<Vector2Int> FromElevationMap(float[,] elevation, Vector2Int startPosition) {
		HashSet<Vector2Int> river = new HashSet<Vector2Int>();
		
		// this is a simple Best-Path-First BFS graph-search system
		// start at a certain elevation, and consistently move downwards, until you reach a local minimum
		
		// init position
		Vector2Int currentPos = startPosition;
				
		PriorityQueue<Vector2Int> pathQueue = new PriorityQueue<Vector2Int>();
		pathQueue.Enqueue(0, currentPos);
		
		// BFS search here
		while (pathQueue.Count != 0) {
			currentPos = pathQueue.Dequeue();
			if (river.Contains(currentPos)) continue;
			
			float currMinEl = elevation[currentPos.x, currentPos.y];
			List<Vector2Int> minVals = new List<Vector2Int>();

			foreach (Vector2Int adjacent in GetNeighbors(elevation, currentPos)) {
				if (elevation[adjacent.x, adjacent.y] <= currMinEl) {
					currMinEl = elevation[adjacent.x, adjacent.y];
					minVals.Add(adjacent);
				}
			}
			foreach (Vector2Int candidate in minVals) {
				if (elevation[candidate.x, candidate.y] == currMinEl) {
					river.Add(candidate);
					pathQueue.Enqueue(0, candidate);
				}
			}
		}

		river.Add(startPosition);
		return river;
	}

	private static IEnumerable<Vector2Int> GetNeighbors(float[,] map, Vector2Int pos) {
		Vector2Int mapBounds = new Vector2Int(map.GetLength(0), map.GetLength(1));

		for (int x = -1; x <= 1; x++) {
			if (pos.x+x < 0 || pos.x+x > mapBounds.x-1) continue;
			for (int y = -1; y <= 1; y++) {
				if (pos.y+y < 0 || pos.y+y > mapBounds.y-1) continue;
				if (x != 0 && y != 0) continue; // discard non-cardinals

				yield return new Vector2Int(x, y) + pos;
			}
		}
	}
}