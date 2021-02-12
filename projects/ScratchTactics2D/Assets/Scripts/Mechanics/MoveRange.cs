using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Extensions;

public class MoveRange : FlowField
{	
	public static MoveRange MoveRangeFrom(Vector3Int origin, HashSet<Vector3Int> nodeSet, int range = Int32.MaxValue) {
		//		
		Dictionary<Vector3Int, int> distance = new Dictionary<Vector3Int, int>();
		PriorityQueue<Vector3Int> fieldQueue = new PriorityQueue<Vector3Int>();

		// initial setup
		Vector3Int currentPos = origin;
		distance[origin] = 0;	
		fieldQueue.Enqueue(0, currentPos);
		
		while (fieldQueue.Count != 0) {
			currentPos = fieldQueue.Dequeue();
					
			foreach (Vector3Int adjacent in GetAdjacent(currentPos)) {
				if (nodeSet.Contains(adjacent)) {
					int distSoFar = (distance.ContainsKey(currentPos)) ? distance[currentPos] : 0;
					var updatedCost = distSoFar + Cost(currentPos, adjacent);
					if (updatedCost > range) { continue; }
					
					if (!distance.ContainsKey(adjacent) || updatedCost < distance[adjacent]) {
						distance[adjacent] = updatedCost;
						fieldQueue.Enqueue(distance[adjacent], adjacent);
					}
				}
			}
		}

		MoveRange mRange = new MoveRange();
		mRange.origin = origin;
		mRange.field = distance;
		return mRange;
	}

	// ValidMoves will indicate what can be passed through,
	// and MoveRange will indicate what must be pathed around
	public bool ValidMove(Vector3Int tilePos) {
		return field.ContainsKey(tilePos) && GameManager.inst.tacticsManager.GetActiveGrid().VacantAt(tilePos);
	}

	public override void Display(GameGrid grid) {
		foreach (Vector3Int tilePos in field.Keys) {
			if (ValidMove(tilePos)) {
				grid.UnderlayAt(tilePos, Utils.selectColorBlue);
			}
		}
	}
}