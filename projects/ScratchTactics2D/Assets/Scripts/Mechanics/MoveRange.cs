using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Extensions;

public class MoveRange : FlowField
{	
	public MoveRange(){}
	public MoveRange(Vector3Int _origin) {
		origin = _origin;
		field = new Dictionary<Vector3Int, int>{
			[_origin] = 0
		};
	}

	// ValidMoves will indicate what can be passed through,
	// and MoveRange will indicate what must be pathed around
	public bool ValidMove(Vector3Int tilePos) {
		return field.ContainsKey(tilePos) && Battle.active.grid.VacantAt(tilePos);
	}

	public override void Display(GameGrid grid) {
		foreach (Vector3Int tilePos in field.Keys) {
			if (ValidMove(tilePos)) {
				grid.UnderlayAt(tilePos, Constants.selectColorBlue);
			}
		}
	}

	public void Display(GameGrid grid, Color colorOverride) {
		foreach (Vector3Int tilePos in field.Keys) {
			if (ValidMove(tilePos)) {
				grid.UnderlayAt(tilePos, colorOverride);
			}
		}
	}
}