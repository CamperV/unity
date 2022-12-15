using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Extensions;

public class TargetRange : FlowField<GridPosition>
{	
	public static TargetRange Empty {
		get {
			TargetRange ar = new TargetRange();
			ar.field = new Dictionary<GridPosition, int>();
			return ar;
		}
	}
	
	public TargetRange(){}
	public TargetRange(MoveRange moveRange, int minRange, int maxRange) {
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

	public static TargetRange Standing(GridPosition gp, int minRange, int maxRange) {
		TargetRange ar = new TargetRange();
		ar.origin = gp;
		ar.field = new Dictionary<GridPosition, int>();
			
		// blossom out until we hit "range"
		foreach (GridPosition withinRange in ar.origin.Radiate(maxRange, min: minRange)) {	
			ar.field[withinRange] = ar.origin.ManhattanDistance(withinRange);
		}
		return ar;
	}

	public static TargetRange OfDimension(GridPosition gp, ICollection<GridPosition> dimensions) {
		TargetRange ar = new TargetRange();
		ar.origin = gp;
		ar.field = new Dictionary<GridPosition, int>();
			
		foreach (GridPosition d in dimensions) {	
			ar.field[d] = -1;
		}
		return ar;
	}


	public bool ValidTarget(GridPosition gp) {
		return gp != origin && field.ContainsKey(gp);
	}

	public void Display(IGrid<GridPosition> target, Color color) {
		foreach (GridPosition tilePos in field.Keys) {
			target.Highlight(tilePos, color);
		}
	}

	public void Display(IGrid<GridPosition> target, Color color, VisualTile vt) {
		foreach (GridPosition tilePos in field.Keys) {
			target.SetHighlightTile(tilePos, vt);
			target.Highlight(tilePos, color);
		}
	}
}