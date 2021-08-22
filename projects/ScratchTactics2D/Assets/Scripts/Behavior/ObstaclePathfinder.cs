using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class ObstaclePathfinder : Pathfinder
{
	protected HashSet<Vector3Int> obstacles;

	public ObstaclePathfinder(HashSet<Vector3Int> _obstacles) {
		obstacles = _obstacles;
	}
	public ObstaclePathfinder() {
		obstacles = new HashSet<Vector3Int>();
	}

	public override T BFS<T>(Vector3Int startPosition, Vector3Int targetPosition) {
		if (obstacles.Contains(targetPosition)) return null;
		bool unreachable = true;

		// this is a simple Best-Path-First BFS graph-search system
		// Grid Positions are the Nodes, and are connected to their neighbors
		
		// init position
		Vector3Int currentPos = startPosition;
		
		// track path creation
		Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
		Dictionary<Vector3Int, int> distance = new Dictionary<Vector3Int, int>();
		
		PriorityQueue<Vector3Int> pathQueue = new PriorityQueue<Vector3Int>();
		pathQueue.Enqueue(0, currentPos);
		
		// BFS search here
		while (pathQueue.Count != 0) {
			currentPos = pathQueue.Dequeue();
			
			// found the target, now recount the path
			if (currentPos == targetPosition) {
				unreachable = false;
				break;
			}
			
			// available positions are: your neighbors that are "moveable",
			// minus any endpoints other pathers have scoped out
			foreach (Vector3Int adjacent in pathableSurface.GetNeighbors(currentPos)) {
				if (obstacles.Contains(adjacent) && adjacent != targetPosition) continue;
				
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
		if (unreachable) {
			Debug.Log($"Got UNREACHABLE. THIS SHOULDN'T HAPPEN!");
			Debug.Assert(false);
			// return this so that the game doesn't crash
			T rp = new T();
			rp.AddFirst(startPosition);
			return rp;
		}
		// else:
		
		// if we found the target, recount the path to get there
		T newPath = new T();
			
		// init value only
		Vector3Int progenitor = targetPosition;
		newPath.AddFirst(targetPosition);

		while (progenitor != startPosition) {
			var newProgenitor = cameFrom[progenitor];
			
			// build the path in reverse, aka next steps (including target)
			newPath.AddFirst(newProgenitor);
			progenitor = newProgenitor;
		}
		return newPath;
	}

	public T FlowField<T>(Vector3Int startPosition, int range = Int32.MaxValue, int numElements = Int32.MaxValue) where T : FlowField, new() {
		Dictionary<Vector3Int, int> distance = new Dictionary<Vector3Int, int>();
		PriorityQueue<Vector3Int> fieldQueue = new PriorityQueue<Vector3Int>();

		// initial setup
		Vector3Int currentPos = startPosition;
		fieldQueue.Enqueue(0, currentPos);
		distance[startPosition] = 0;
		
		while (fieldQueue.Count != 0) {
			currentPos = fieldQueue.Dequeue();
					
			foreach (Vector3Int adjacent in pathableSurface.GetNeighbors(currentPos)) {
				if (distance.Count > numElements) continue;
				if (obstacles.Contains(adjacent)) continue;
				int costAt = pathableSurface.EdgeCost(currentPos, adjacent);
				if (costAt == -1) continue;	// -1 indicates this area is impassable
				
				int distSoFar = (distance.ContainsKey(currentPos)) ? distance[currentPos] : 0;
				var updatedCost = distSoFar + costAt;
				if (updatedCost > range) continue;
				
				if (!distance.ContainsKey(adjacent) || updatedCost < distance[adjacent]) {
					distance[adjacent] = updatedCost;
					fieldQueue.Enqueue(distance[adjacent], adjacent);
				}
			}
		}
		
		// upon success, create and return a FlowField using the distance dict
		T flowField = new T();
		flowField.origin = startPosition;
		flowField.field = distance;
		return flowField;
	}
}
