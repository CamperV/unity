using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class FlowField<T> where T : struct
{
	public T origin;
	public Dictionary<T, int> field;
	
	public FlowField(){}
	public FlowField(T _origin, Dictionary<T, int> _field) {
		origin = _origin;
		field = _field;
	}
	
	public void Absorb(FlowField<T> other) {
		// this method simply takes an old FF, and combines the dictionaries
		Dictionary<T, int> mergedField = new Dictionary<T, int>(other.field);
		
		// overwrite any old keys, but keep old stale keys
		foreach(T pos in field.Keys) {
			mergedField[pos] = field[pos];
		}
		
		field = mergedField;
	}
	
	// public virtual void Display(GameGrid grid) {
	// 	foreach (Vector3Int tilePos in field.Keys) {
	// 		grid.UnderlayAt(tilePos, Constants.threatColorRed);
	// 	}
	// }

	// public virtual void ClearDisplay(GameGrid grid) {
	// 	foreach (Vector3Int tilePos in field.Keys) {
	// 		grid.ResetUnderlayAt(tilePos);
	// 	}
	// }
}