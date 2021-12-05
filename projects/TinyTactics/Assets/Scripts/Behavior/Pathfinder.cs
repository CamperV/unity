using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class Pathfinder<T> where T : struct
{
	public IPathable<T> pathableSurface;

	public Pathfinder(){}
	public Pathfinder(IPathable<T> _pathableSurface) {
		pathableSurface = _pathableSurface;
	}

	public Path<T>? BFS(T startPosition, T targetPosition) {
		// this is a simple Best-Path-First BFS graph-search system
		// Grid Positions are the Nodes, and are connected to their neighbors
		
		// init position
		T currentPos = startPosition;
		
		// track path creation
		Dictionary<T, T> cameFrom = new Dictionary<T, T>();
		Dictionary<T, int> distance = new Dictionary<T, int>();
		bool foundTarget = false;
		
		PriorityQueue<T> pathQueue = new PriorityQueue<T>();
		pathQueue.Enqueue(0, currentPos);
		
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
			foreach (T adjacent in pathableSurface.GetNeighbors(currentPos)) {
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

		// if it proved impossible to find a path, return null
		if (!foundTarget) {
			return null;

		// if we found the target, recount the path to get there
		} else {
			Path<T> newPath = new Path<T>();
					
			// init value only
			T progenitor = targetPosition;
			newPath.AddFirst(targetPosition); // space just outside of the target

			while (!progenitor.Equals(startPosition)) {
				T newProgenitor = cameFrom[progenitor];
				
				// build the path in reverse, aka next steps (including target)
				newPath.AddFirst(newProgenitor);
				progenitor = newProgenitor;
			}

			return newPath;
		}
	}

	public U GenerateFlowField<U>(T startPosition, int range = Int32.MaxValue, int numElements = Int32.MaxValue) where U : FlowField<T>, new() {
		Dictionary<T, int> distance = new Dictionary<T, int>();
		PriorityQueue<T> fieldQueue = new PriorityQueue<T>();

		// initial setup
		T currentPos = startPosition;
		fieldQueue.Enqueue(0, currentPos);
		distance[startPosition] = 0;
		
		while (fieldQueue.Count != 0) {
			currentPos = fieldQueue.Dequeue();
					
			foreach (T adjacent in pathableSurface.GetNeighbors(currentPos)) {
				if (distance.Count > numElements) continue;

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
		U flowField = new U();
		flowField.origin = startPosition;
		flowField.field = distance;
		return flowField;
	}
}
