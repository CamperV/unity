using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Extensions;

public class AttackRange : FlowField<GridPosition>
{	
	public AttackRange(){}
	public AttackRange(MoveRange moveRange, int range) {
		origin = moveRange.origin;
		field = new Dictionary<GridPosition, int>(moveRange.field);

    	foreach (GridPosition standingPos in moveRange.field.Keys) {				
            // TODO: there are some inefficiences here, but do we really care?
            // blossom out until we hit "range"
            foreach (GridPosition withinRange in standingPos.Radiate(range)) {	
                if (moveRange.field.ContainsKey(withinRange)) continue;

                // if you're outside of the moveable range, add your cost via Manhattan Dist to the
                // pre-existing MoveRange cost
                field[withinRange] = moveRange.field[standingPos] + standingPos.ManhattanDistance(withinRange);
            }
		}
	}

	// public bool ValidAttack(Unit currentSelection, Vector3Int tilePos) {
	// 	bool withinRange = tilePos.ManhattanDistance(currentSelection.gridPosition) <= currentSelection._RANGE;
	// 	return field.ContainsKey(tilePos) && withinRange;
	// }

	public void Display(IGrid<GridPosition> target) {
		foreach (GridPosition tilePos in field.Keys) {
			target.Highlight(tilePos, Constants.threatColorRed);
		}
	}

	public void ClearDisplay(IGrid<GridPosition> target) {
		target.ResetHighlight();
	}
}