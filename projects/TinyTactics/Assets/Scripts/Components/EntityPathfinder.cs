using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class EntityPathfinder : MonoBehaviour
{
	private BattleMap battleMap;

	// get your own IPathable
	void Awake(){
		Battle _topBattleRef = GetComponentInParent<Battle>();
		battleMap = _topBattleRef.GetComponentInChildren<BattleMap>();
	}

	public Path<GridPosition>? BFS(GridPosition startPosition, GridPosition targetPosition) {
		// this is a simple Best-Path-First BFS graph-search system
		// Grid Positions are the Nodes, and are connected to their neighbors
		
		// init position
		GridPosition currentPos = startPosition;
		
		// track path creation
		Dictionary<GridPosition, GridPosition> cameFrom = new Dictionary<GridPosition, GridPosition>();
		Dictionary<GridPosition, int> distance = new Dictionary<GridPosition, int>();
		bool foundTarget = false;
		
		PriorityQueue<GridPosition> pathQueue = new PriorityQueue<GridPosition>();
		pathQueue.Enqueue(0, currentPos);

		TerrainCostOverride tco = GetComponent<TerrainCostOverride>();
		
		// BFS search here
		while (pathQueue.Count != 0) {
			currentPos = pathQueue.Dequeue();
			
			// found the target, now recount the path
			if (currentPos.Equals(targetPosition)) {
				foundTarget = true;
				break;
			}
			
			// available positions are: your neighbors that are "moveable",
			// minus any endpoints other pathers have scoped out
			foreach (GridPosition adjacent in battleMap.GetNeighbors(currentPos)) {
				int costAt = (tco == null) ? battleMap.BaseCost(adjacent) : battleMap.OverrideCost(adjacent, tco);
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

		// if it proved impossible to find a path, return null
		if (!foundTarget) {
			return null;

		// if we found the target, recount the path to get there
		} else {
			Path<GridPosition> newPath = new Path<GridPosition>();
					
			// init value only
			GridPosition progenitor = targetPosition;
			newPath.AddFirst(targetPosition); // space just outside of the target

			while (!progenitor.Equals(startPosition)) {
				GridPosition newProgenitor = cameFrom[progenitor];
				
				// build the path in reverse, aka next steps (including target)
				newPath.AddFirst(newProgenitor);
				progenitor = newProgenitor;
			}

			return newPath;
		}
	}

	public T GenerateFlowField<T>(GridPosition startPosition, int range = Int32.MaxValue, int numElements = Int32.MaxValue) where T : FlowField<GridPosition>, new() {
		Dictionary<GridPosition, int> distance = new Dictionary<GridPosition, int>();
		PriorityQueue<GridPosition> fieldQueue = new PriorityQueue<GridPosition>();

		// initial setup
		GridPosition currentPos = startPosition;
		fieldQueue.Enqueue(0, currentPos);
		distance[startPosition] = 0;

		// if a TCO exists, use its values when applicable
		TerrainCostOverride tco = GetComponent<TerrainCostOverride>();
		
		while (fieldQueue.Count != 0) {
			currentPos = fieldQueue.Dequeue();
					
			foreach (GridPosition adjacent in battleMap.GetNeighbors(currentPos)) {
				if (distance.Count > numElements) continue;

				// int costAt = battleMap.BaseCost(adjacent);
				int costAt = (tco == null) ? battleMap.BaseCost(adjacent) : battleMap.OverrideCost(adjacent, tco);
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
