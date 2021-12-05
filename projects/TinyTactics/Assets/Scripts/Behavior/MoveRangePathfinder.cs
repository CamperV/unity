using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MoveRangePathfinder : Pathfinder<GridPosition>
{
	public MoveRangePathfinder(MoveRange moveRange) {
		pathableSurface = moveRange;
	}
	
    public Path<GridPosition>? BFS(GridPosition startPosition, GridPosition targetPosition) {

        // we can short-circuit easily - if the MoveRange doesn't have the key, don't even try
        if (!(pathableSurface as MoveRange).ValidMove(targetPosition)) {
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
			if (currentPos == (pathableSurface as MoveRange).origin) break;

			GridPosition bestMove = currentPos;
			int bestMoveCost = (pathableSurface as MoveRange).field[currentPos];
			
			foreach (GridPosition adjacent in pathableSurface.GetNeighbors(currentPos)) {		
				int cost = pathableSurface.EdgeCost(currentPos, adjacent);

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
