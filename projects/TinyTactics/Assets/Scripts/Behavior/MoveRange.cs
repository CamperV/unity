using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Extensions;

public class MoveRange : FlowField<GridPosition>, IPathable<GridPosition>
{	
	public MoveRange(){}
	public MoveRange(GridPosition _origin) {
		origin = _origin;
		field = new Dictionary<GridPosition, int>{
			[_origin] = 0
		};
	}

	// ValidMoves will indicate what can be passed through,
	// and MoveRange will indicate what must be pathed around
	public bool ValidMove(GridPosition tilePos) {
		return field.ContainsKey(tilePos);
	}

	// IPathable definitions
    public IEnumerable<GridPosition> GetNeighbors(GridPosition origin) {
        GridPosition up    = origin + (GridPosition)Vector2Int.up;
        GridPosition right = origin + (GridPosition)Vector2Int.right;
        GridPosition down  = origin + (GridPosition)Vector2Int.down;
        GridPosition left  = origin + (GridPosition)Vector2Int.left;
        if (field.ContainsKey(up))    yield return up;
        if (field.ContainsKey(right)) yield return right;
        if (field.ContainsKey(down))  yield return down;
        if (field.ContainsKey(left))  yield return left;
    }

	public int EdgeCost(GridPosition src, GridPosition dest) {
		return field[dest];
	}

	public void Display(IGrid<GridPosition> target) {
		foreach (GridPosition tilePos in field.Keys) {
			if (ValidMove(tilePos)) {
				target.Highlight(tilePos, Constants.selectColorBlue);
			}
		}
	}

	public void ClearDisplay(IGrid<GridPosition> target) {
		target.ResetHighlight();
	}
}