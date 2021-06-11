using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class FlowField : IPathable
{
	public Vector3Int origin;
	public Dictionary<Vector3Int, int> field;
	
	public FlowField(){}
	public FlowField(Vector3Int _origin, Dictionary<Vector3Int, int> _field) {
		origin = _origin;
		field = _field;
	}

	// IPathable definitions
	public IEnumerable<Vector3Int> GetNeighbors(Vector3Int pos) {
		List<Vector3Int> options = new List<Vector3Int>() {
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

		foreach (Vector3Int opt in options) {
			if (field.ContainsKey(opt))
				yield return opt;
		}
	}
	public int EdgeCost(Vector3Int src, Vector3Int dest) {
		return field[dest];
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
		foreach (var k in field.Keys) {
			GameManager.inst.overworld.ResetHighlightTile(k);
		}
		
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
			
			foreach (var v in toHighlight[mag]) {
				GameManager.inst.overworld.HighlightTile(v, color);
			}
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