using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public interface IPathable
{
	IEnumerable<Vector3Int> GetNeighbors(Vector3Int v);
	int EdgeCost(Vector3Int src, Vector3Int dest);
}

public abstract class Pathfinder
{
	public IPathable pathableSurface;

	public virtual T BFS<T>(Vector3Int startPosition, Vector3Int targetPosition) where T : Path, new() {
		// this is a simple Best-Path-First BFS graph-search system
		// Grid Positions are the Nodes, and are connected to their neighbors
		
		// init position
		Vector3Int currentPos = startPosition;
		
		// track path creation
		Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
		Dictionary<Vector3Int, int> distance = new Dictionary<Vector3Int, int>();
		bool foundTarget = false;
		
		PriorityQueue<Vector3Int> pathQueue = new PriorityQueue<Vector3Int>();
		pathQueue.Enqueue(0, currentPos);
		
		// BFS search here
		while (pathQueue.Count != 0) {
			currentPos = pathQueue.Dequeue();
			
			// found the target, now recount the path
			if (currentPos == targetPosition) {
				foundTarget = true;
				break;
			}
			
			// available positions are: your neighbors that are "moveable",
			// minus any endpoints other pathers have scoped out
			foreach (Vector3Int adjacent in pathableSurface.GetNeighbors(currentPos)) {
				int costAt = pathableSurface.EdgeCost(currentPos, adjacent);
				if (costAt == -1) continue;	// -1 indicates this area is impassable

				// units can move through units of similar types, but not enemy types
				int distSoFar = (distance.ContainsKey(currentPos)) ? distance[currentPos] : 0;
				int updatedCost = distSoFar + costAt;
				
				if (!distance.ContainsKey(adjacent) || updatedCost < distance[adjacent]) {
					distance[adjacent] = updatedCost;
					cameFrom[adjacent] = currentPos;
					pathQueue.Enqueue(distance[adjacent], adjacent);
				}
			}
		}
		
		// if we found the target, recount the path to get there
		T newPath = new T();
		
		if (foundTarget) {		
			// init value only
			Vector3Int progenitor = targetPosition;
			newPath.AddFirst(targetPosition); // space just outside of the target

			while (progenitor != startPosition) {
				var newProgenitor = cameFrom[progenitor];
				
				// build the path in reverse, aka next steps (including target)
				newPath.AddFirst(newProgenitor);
				progenitor = newProgenitor;
			}
		}
		return newPath;
	}
}
