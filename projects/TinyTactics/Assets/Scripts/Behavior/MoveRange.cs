using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Extensions;

public class MoveRange : FlowField<GridPosition>, IPathable<GridPosition>
{	
	// this list is invoked to determine all rules that allow a ValidMove to exist
	// MoveRange users will add to this list, this class will execute
	private List<Func<GridPosition, bool>> _ValidMoveFuncPool = new List<Func<GridPosition, bool>>();

	public MoveRange(){}
	public MoveRange(GridPosition _origin) {
		origin = _origin;
		field = new Dictionary<GridPosition, int>{
			[_origin] = 0
		};
	}

	public void RegisterValidMoveFunc(Func<GridPosition, bool> Func) {
		_ValidMoveFuncPool.Add(Func);
	}

	// ValidMoves will indicate what can be passed through,
	// and MoveRange will indicate what must be pathed around
	public bool ValidMove(GridPosition gp) {
		bool valid = field.ContainsKey(gp);

		foreach (var Func in _ValidMoveFuncPool) {
			valid &= Func(gp);
		}
		return valid;
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

	public int BaseCost(GridPosition gp) {
		return field[gp];
	}

	public void Display(IGrid<GridPosition> target) {
		foreach (GridPosition tilePos in field.Keys) {
			if (ValidMove(tilePos)) {
				target.Highlight(tilePos, Constants.selectColorBlue);
			}
		}
	}

    public Path<GridPosition>? BFS(GridPosition startPosition, GridPosition targetPosition) {
        // we can short-circuit easily - if the MoveRange doesn't have the key, don't even try
        if (!ValidMove(targetPosition)) {
            return null;
        }

		// this BFS moves backwards, from a targetPosition into a FlowField's origin
		// init position
		GridPosition currentPos = targetPosition;

		// this will be built while we traverse
		Path<GridPosition> newPath = new Path<GridPosition>();
		newPath.AddFirst(currentPos);
		
		PriorityQueue<GridPosition> pathQueue = new PriorityQueue<GridPosition>();
		pathQueue.Enqueue(0, currentPos);
		
		// BFS search here
		while (pathQueue.Count != 0) {
			currentPos = pathQueue.Dequeue();
			
			// found the target, now recount the path
			if (currentPos == origin) break;

			GridPosition bestMove = currentPos;
			int bestMoveCost = field[currentPos];
			
			foreach (GridPosition adjacent in GetNeighbors(currentPos)) {		
				int cost = BaseCost(adjacent);

				if (cost < bestMoveCost) {
					bestMoveCost = cost;
					bestMove = adjacent;
				}
			}

			// keep prepending the path to build it backwards
			if (bestMove != currentPos) {
				pathQueue.Enqueue(bestMoveCost, bestMove);
				newPath.AddFirst(bestMove);
			}
		}

		return newPath;
	}
}