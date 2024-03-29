﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Extensions;

public class AttackRange : FlowField
{	
	public AttackRange(){}
	public AttackRange(MoveRange mRange, int range) {
		origin = mRange.origin;
		field = new Dictionary<Vector3Int, int>(mRange.field);	

		//		
		PriorityQueue<Vector3Int> fieldQueue = new PriorityQueue<Vector3Int>();
		
		foreach (Vector3Int tilePos in mRange.field.Keys) {
			fieldQueue.Enqueue(0, tilePos);
			Dictionary<Vector3Int, int> distance = new Dictionary<Vector3Int, int>();

			while (fieldQueue.Count != 0) {
				Vector3Int currentPos = fieldQueue.Dequeue();
				
				// TODO: there are some inefficiences here, but do we really care?
				// blossom out until we hit "range"
				foreach (Vector3Int adjacent in currentPos.GridRadiate(Battle.active.grid, 1)) {	
					int distSoFar = (distance.ContainsKey(currentPos)) ? distance[currentPos] : 0;
					var updatedCost = distSoFar + 1;
					if (updatedCost > range) continue;
					
					if (!distance.ContainsKey(adjacent) || updatedCost < distance[adjacent]) {
						distance[adjacent] = updatedCost;
						fieldQueue.Enqueue(distance[adjacent], adjacent);
					}
				}
			}

			// update the field to include the "blossom"
			foreach (var k in distance.Keys) {
				if (!field.ContainsKey(k)) {
					field[k] = distance[k];
				}
			}
		}
	}

	public bool ValidAttack(Unit currentSelection, Vector3Int tilePos) {
		bool withinRange = tilePos.ManhattanDistance(currentSelection.gridPosition) <= currentSelection._RANGE;
		return field.ContainsKey(tilePos) && withinRange;
	}

	public override void Display(GameGrid grid) {
		foreach (Vector3Int tilePos in field.Keys) {
			grid.UnderlayAt(tilePos, Constants.threatColorRed);
		}
	}
}