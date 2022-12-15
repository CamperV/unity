using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class GenericPathfinder<T> where T : struct
{
	private IPathable<T> pathableSurface;

	public GenericPathfinder(IPathable<T> _pathableSurface) {
		pathableSurface = _pathableSurface;
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

				int costAt = pathableSurface.BaseCost(adjacent);
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
