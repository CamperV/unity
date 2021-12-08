using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Extensions;

public class AttackRange : FlowField<GridPosition>
{	
	public AttackRange(){}
	public AttackRange(MoveRange moveRange, int minRange, int maxRange) {
		origin = moveRange.origin;
		field = new Dictionary<GridPosition, int>(moveRange.field);

    	foreach (GridPosition standingPos in moveRange.field.Keys) {
			if (!moveRange.ValidMove(standingPos)) continue;

            // blossom out until we hit "range"
            foreach (GridPosition withinRange in standingPos.Radiate(maxRange, min: minRange)) {	
                if (moveRange.field.ContainsKey(withinRange)) continue;

                // if you're outside of the moveable range, add your cost via Manhattan Dist to the
                // pre-existing MoveRange cost
                field[withinRange] = moveRange.field[standingPos] + standingPos.ManhattanDistance(withinRange);
            }
		}

		field.Remove(origin);
	}

	public bool ValidAttack(GridPosition tilePos) {
		return field.ContainsKey(tilePos);
	}

	public void Display(IGrid<GridPosition> target) {
		foreach (GridPosition tilePos in field.Keys) {
			target.Highlight(tilePos, Constants.threatColorRed);
		}
	}
}