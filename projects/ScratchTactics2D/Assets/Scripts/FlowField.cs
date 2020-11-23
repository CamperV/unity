using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class FlowField
{
	public Vector3Int origin;
	public Dictionary<Vector3Int, int> field;
	
	public static FlowField FlowFieldFrom(Vector3Int origin, HashSet<Vector3Int> nodeSet, int range = Int32.MaxValue, int numElements = Int32.MaxValue) {
		//		
		Dictionary<Vector3Int, int> distance = new Dictionary<Vector3Int, int>();
		PriorityQueue<Vector3Int> fieldQueue = new PriorityQueue<Vector3Int>();

		// initial setup
		Vector3Int currentPos = origin;		
		fieldQueue.Enqueue(0, currentPos);
		
		while (fieldQueue.Count != 0) {
			currentPos = fieldQueue.Dequeue();
					
			foreach (Vector3Int adjacent in GetAdjacent(currentPos)) {
				if (distance.Count > numElements) { continue; }
				
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
		
		// upon success, create and return a FlowField using the distance dict
		FlowField newField = new FlowField();
		newField.origin = origin;
		newField.field = distance;
		return newField;
	}
	
	private static List<Vector3Int> GetAdjacent(Vector3Int pos) {
		return new List<Vector3Int>() {
			pos + Vector3Int.up,	// N
			pos + Vector3Int.right,	// E
			pos + Vector3Int.down,	// S
			pos + Vector3Int.left	// W
		};
	}
	
	private static int Cost(Vector3Int src, Vector3Int dest) {
		// the way we have coded cost into WorldTile:
		// the number listed is the cost to enter said tile
		var destTile = GameManager.inst.GetActiveGrid().GetTileAt(dest);
		return destTile.cost;
	}
	
	public void Absorb(FlowField other) {
		// this method simply takes an old FF, and combines the dictionaries
		Dictionary<Vector3Int, int> mergedField = new Dictionary<Vector3Int, int>(other.field);
		
		// overwrite any old keys, but keep old stale keys
		foreach(Vector3Int pos in field.Keys) {
			mergedField[pos] = field[pos];
		}
		
		field = mergedField;
	}
	
	// just grabs worldGrid for prototyping
	public void Display() {
		GameManager.inst.worldGrid.ResetHighlightTiles(new HashSet<Vector3Int>(field.Keys));
		
		Dictionary<int, HashSet<Vector3Int>> toHighlight = new Dictionary<int, HashSet<Vector3Int>>();
		
		foreach(Vector3Int pos in field.Keys) {
			int mag = field[pos];
			if (!toHighlight.ContainsKey(mag)) {
				toHighlight[mag] = new HashSet<Vector3Int>();
			}
			toHighlight[mag].Add(pos);
		}
		foreach(int mag in toHighlight.Keys) {
			float scale = .05f;
			Color color = new Color(1f-(scale*mag), 1f-(scale*mag), 1f-(scale*mag));
			
			GameManager.inst.worldGrid.HighlightTiles(toHighlight[mag], color);
		}
	}
}