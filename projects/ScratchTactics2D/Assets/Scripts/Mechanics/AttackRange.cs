using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Extensions;

public class AttackRange : FlowField
{	
	public static AttackRange AttackRangeFrom(MoveRange mRange, HashSet<Vector3Int> nodeSet, int range = 1) {
		AttackRange aRange = new AttackRange();
		aRange.origin = mRange.origin;
		aRange.field = new Dictionary<Vector3Int, int>(mRange.field);
		//		
		PriorityQueue<Vector3Int> fieldQueue = new PriorityQueue<Vector3Int>();
		
		foreach (Vector3Int tilePos in mRange.field.Keys) {
			fieldQueue.Enqueue(0, tilePos);
			Dictionary<Vector3Int, int> distance = new Dictionary<Vector3Int, int>();

			while (fieldQueue.Count != 0) {
				Vector3Int currentPos = fieldQueue.Dequeue();
				
				// TODO: there are some inefficiences here, but do we really care?
				// blossom out until we hit "range"
				//foreach (Vector3Int adjacent in GetAdjacent(currentPos)) {
				foreach (Vector3Int adjacent in currentPos.GridRadiate(GameManager.inst.GetActiveGrid(), 1)) {	
					if (nodeSet.Contains(adjacent)) {
						int distSoFar = (distance.ContainsKey(currentPos)) ? distance[currentPos] : 0;
						var updatedCost = distSoFar + 1;
						if (updatedCost > range) { continue; }
						
						if (!distance.ContainsKey(adjacent) || updatedCost < distance[adjacent]) {
							distance[adjacent] = updatedCost;
							fieldQueue.Enqueue(distance[adjacent], adjacent);
						}
					}
				}
			}

			// update the field to include the "blossom"
			foreach (var k in distance.Keys) {
				if (!aRange.field.ContainsKey(k)) {
					aRange.field[k] = distance[k];
				}
			}
		}

		return aRange;
	}

	// ValidMoves will indicate what can be passed through,
	// and MoveRange will indicate what must be pathed around
	public bool ValidAttack(Unit currentSelection, Vector3Int tilePos) {
		bool withinRange = tilePos.ManhattanDistance(currentSelection.gridPosition) <= currentSelection._RANGE;
		return field.ContainsKey(tilePos) && withinRange;
	}
}