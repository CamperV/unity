using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class FlowFieldPathfinder : Pathfinder
{
	public FlowFieldPathfinder(FlowField field) {
		pathableSurface = field;
	}
	
	public T BFS<T>(Vector3Int targetPosition) where T : Path, new() {
		// this BFS moves backwards, from a targetPosition into a FlowField's origin
		// init position
		Vector3Int currentPos = targetPosition;

		// this will be built while we traverse
		T newPath = new T();
		newPath.AddFirst(currentPos);
		
		PriorityQueue<Vector3Int> pathQueue = new PriorityQueue<Vector3Int>();
		pathQueue.Enqueue(0, currentPos);
		
		// BFS search here
		while (pathQueue.Count != 0) {
			currentPos = pathQueue.Dequeue();
			
			// found the target, now recount the path
			if (currentPos == (pathableSurface as FlowField).origin) break;

			Vector3Int bestMove = currentPos;
			int bestMoveCost = (pathableSurface as FlowField).field[currentPos];
			
			foreach (Vector3Int adjacent in pathableSurface.GetNeighbors(currentPos)) {		
				int cost = pathableSurface.EdgeCost(currentPos, adjacent);

				if (cost < bestMoveCost) {
					bestMoveCost = cost;
					bestMove = adjacent;
				}
			}

			// keep prepending the path to build it backwards
			if (bestMove != currentPos) {
				pathQueue.Enqueue(bestMoveCost, bestMove);
				newPath.AddFirst(bestMove);
			}
		}

		return newPath;
	}
}
