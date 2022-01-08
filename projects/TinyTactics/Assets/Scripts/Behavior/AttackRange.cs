using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Extensions;

public class AttackRange : FlowField<GridPosition>
{	
	public static AttackRange Empty {
		get {
			AttackRange ar = new AttackRange();
			ar.field = new Dictionary<GridPosition, int>();
			return ar;
		}
	}
	
	public AttackRange(){}
	public AttackRange(MoveRange moveRange, int minRange, int maxRange) {
		origin = moveRange.origin;
		field = new Dictionary<GridPosition, int>(moveRange.field);

    	foreach (GridPosition standingPos in moveRange.field.Keys) {
			
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

	public static AttackRange Standing(GridPosition gp, int minRange, int maxRange) {
		AttackRange ar = new AttackRange();
		ar.origin = gp;
		ar.field = new Dictionary<GridPosition, int>();
			
		// blossom out until we hit "range"
		foreach (GridPosition withinRange in ar.origin.Radiate(maxRange, min: minRange)) {	
			ar.field[withinRange] = ar.origin.ManhattanDistance(withinRange);
		}
		return ar;
	}

	public static AttackRange OfDimension(GridPosition gp, ICollection<GridPosition> dimensions) {
		AttackRange ar = new AttackRange();
		ar.origin = gp;
		ar.field = new Dictionary<GridPosition, int>();
			
		foreach (GridPosition d in dimensions) {	
			ar.field[d] = -1;
		}
		return ar;
	}


	public bool ValidAttack(GridPosition gp) {
		return gp != origin && field.ContainsKey(gp);
	}

	public void Display(IGrid<GridPosition> target) {
		foreach (GridPosition tilePos in field.Keys) {
			target.Highlight(tilePos, Constants.threatColorRed);
		}
	}
}