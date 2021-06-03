using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class FlowField
{
	public Vector3Int origin;
	public Dictionary<Vector3Int, int> field;
	
	public static FlowField FlowFieldFrom(Vector3Int origin, IEnumerable<Vector3Int> _nodeSet, int range = Int32.MaxValue, int numElements = Int32.MaxValue) {
		HashSet<Vector3Int> nodeSet = new HashSet<Vector3Int>(_nodeSet);

		Dictionary<Vector3Int, int> distance = new Dictionary<Vector3Int, int>();
		PriorityQueue<Vector3Int> fieldQueue = new PriorityQueue<Vector3Int>();

		// initial setup
		Vector3Int currentPos = origin;
		fieldQueue.Enqueue(0, currentPos);
		if (nodeSet.Contains(origin)) {
			distance[origin] = 0;
		}
		
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
	
	public static List<Vector3Int> GetAdjacent(Vector3Int pos) {
		return new List<Vector3Int>() {
			pos + Vector3Int.up,	// N
			pos + Vector3Int.right,	// E
			pos + Vector3Int.down,	// S
			pos + Vector3Int.left,	// W
			pos + new Vector3Int(0, 0, 1)  + Vector3Int.up,
			pos + new Vector3Int(0, 0, 1)  + Vector3Int.right,
			pos + new Vector3Int(0, 0, 1)  + Vector3Int.down,
			pos + new Vector3Int(0, 0, 1)  + Vector3Int.left,
			pos + new Vector3Int(0, 0, -1) + Vector3Int.up,
			pos + new Vector3Int(0, 0, -1) + Vector3Int.right,
			pos + new Vector3Int(0, 0, -1) + Vector3Int.down,
			pos + new Vector3Int(0, 0, -1) + Vector3Int.left,
		};
	}

	public static int Cost(Vector3Int src, Vector3Int dest) {
		// the way we have coded cost into Terrain:
		// the number listed is the cost to enter said tile
		var destTerrain = GameManager.inst.overworld.TerrainAt(dest);
		return destTerrain.cost + (destTerrain.cost * Mathf.Max(dest.z - src.z, 0));
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
	
	// just grabs overworld for prototyping
	public void DebugDisplay() {
		GameManager.inst.overworld.ResetHighlightTiles(new HashSet<Vector3Int>(field.Keys));
		
		Dictionary<float, HashSet<Vector3Int>> toHighlight = new Dictionary<float, HashSet<Vector3Int>>();
		
		foreach(Vector3Int pos in field.Keys) {
			float mag = field[pos] / 100.0f;
			if (!toHighlight.ContainsKey(mag)) {
				toHighlight[mag] = new HashSet<Vector3Int>();
			}
			toHighlight[mag].Add(pos);
		}
		foreach(float mag in toHighlight.Keys) {
			float scale = 0.05f;
			Color color = new Color(1f-(scale*mag), 1f-(scale*mag), 1f-(scale*mag));
			
			GameManager.inst.overworld.HighlightTiles(toHighlight[mag], color);
		}
	}

	public virtual void Display(GameGrid grid) {
		foreach (Vector3Int tilePos in field.Keys) {
			grid.UnderlayAt(tilePos, Constants.threatColorRed);
		}
	}

	public virtual void ClearDisplay(GameGrid grid) {
		foreach (Vector3Int tilePos in field.Keys) {
			grid.ResetUnderlayAt(tilePos);
		}
	}
}