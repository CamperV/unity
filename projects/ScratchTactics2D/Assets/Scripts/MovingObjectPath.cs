using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MovingObjectPath
{
	private Vector3Int _start;
	private Vector3Int _end;
	private PathOverlayTile pathOverlayTile;
	private EndpointOverlayTile endpointOverlayTile;
	
	public Dictionary<Vector3Int, Vector3Int> path = new Dictionary<Vector3Int, Vector3Int>();
	public List<Vector3Int> pathList = new List<Vector3Int>();
	public Vector3Int start {
		get { return _start; }
		set { _start = value; }
	}
	public Vector3Int end {
		get { return _end; }
		set { _end = value; }
	}
	
	public MovingObjectPath(Vector3Int startPosition) {
		start = startPosition;
		
		// load overlay scriptable
		pathOverlayTile = ScriptableObject.CreateInstance<PathOverlayTile>() as PathOverlayTile;
		endpointOverlayTile = ScriptableObject.CreateInstance<EndpointOverlayTile>() as EndpointOverlayTile;
	}
	
	public void Clear() {
		if (path.Count != 0) {
			//ResetDrawPath();
			path.Clear();
			pathList.Clear();
			start = new Vector3Int(-1, -1, -1);
			end   = new Vector3Int(-1, -1, -1);
		}
	}
	
	public Vector3Int Next(Vector3Int position) {
		if (position == end) return end;
		return path[position];
	}
	
	public Vector3Int PopNext(Vector3Int position) {
		// hate that we have to double-search here, but hey, its the API
		// no available Remove(key, out val) override in Unity
		Vector3Int retval = path[position];
		path.Remove(position);
		return retval;
	}

	public IEnumerable<Vector3Int> Unwind(int slice = 0) {
		Vector3Int pos = start;
		do {
			pos = path[pos];
			
			// skip a certain number of tiles when unwinding
			if (slice > 0) {
				slice--;
				continue;
			}
			yield return pos;
		} while (pos != end);
	}
	
	public void Consume(Vector3Int position) {
		path.Remove(position);
		GameManager.inst.worldGrid.ClearOverlayTiles();
	}
	
	public List<Vector3Int> GetPathEdges() {
		List<Vector3Int> edges = new List<Vector3Int>();
		Vector3Int currPos = start;
		Vector3Int nextPos = currPos;
		
		while(nextPos != end) {
			nextPos = path[currPos];
			edges.Add(nextPos - currPos);
			currPos = nextPos;
		}
		return edges;
	}
	
	public bool IsEmpty() {
		return path.Count == 0 || path == null;
	}
	
	public bool IsValid() {
		// check the endpoints to ensure path is still valid
		return !IsEmpty() && GameManager.inst.worldGrid.VacantAt(end);
	}
	
	public void DrawPath() {
		foreach (Vector3Int tile in path.Keys) {
			GameManager.inst.worldGrid.OverlayAt(tile, pathOverlayTile);
		}
		GameManager.inst.worldGrid.OverlayAt(end, endpointOverlayTile);
	}
	
	public void ResetDrawPath() {
		foreach (Vector3Int tile in path.Keys) {
			GameManager.inst.worldGrid.ResetOverlayAt(tile);
		}
		GameManager.inst.worldGrid.ResetOverlayAt(end);
	}

	public void Show(GameGrid grid, OverlayTile overlayTile) {
		foreach (Vector3Int tilePos in Unwind()) {
			if (tilePos == end) break; // skip end tile for debug
			
			grid.OverlayAt(tilePos, overlayTile);
		}
	}

	public void UnShow(GameGrid grid) {
		// slice 1 will clip the start position out
		foreach (Vector3Int tilePos in Unwind()) {
			grid.ResetOverlayAt(tilePos);
		}
	}
	
	public static MovingObjectPath Clip(MovingObjectPath mPath, MoveRange mRange) {
		// unwind until out of bounds, then start whackin'
		List<Vector3Int> forRemoval = new List<Vector3Int>();
		foreach (var pos in mPath.Unwind()) {
			if (!mRange.field.ContainsKey(mPath.Next(pos))) {
				forRemoval.Add(pos);
			}
		}

		// use two loops to avoid modifying path during iteration
		foreach (var fr in forRemoval) {
			mPath.path.Remove(fr);
		}
		mPath.CalcStartEnd();
		return mPath;
	}
		
	// AI pathfinding
	// storing this is a hashmap also helps for quickly assessing what squares are available
	// T is the type you expect to path towards, and don't mind a collision against
	public static MovingObjectPath GetPathTo(Vector3Int startPosition, Vector3Int targetPosition, HashSet<Vector3Int> obstacles) {
		MovingObjectPath newPath = new MovingObjectPath(startPosition);
		
		// this is a simple Best-Path-First BFS graph-search system
		// Grid Positions are the Nodes, and are connected to their neighbors
		
		// init position
		Vector3Int currentPos = startPosition;
		
		// track path creation
		Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
		Dictionary<Vector3Int, int> distance = new Dictionary<Vector3Int, int>();
		bool foundTarget = false;
		
		PriorityQueue<Vector3Int> pathQueue = new PriorityQueue<Vector3Int>();
		pathQueue.Enqueue(0, currentPos);
		
		// BFS search here
		while (pathQueue.Count != 0) {
			currentPos = pathQueue.Dequeue();
			
			// found the target, now recount the path
			if (currentPos == targetPosition) {
				foundTarget = true;
				break;
			}
			
			// available positions are: your neighbors that are "moveable",
			// minus any endpoints other pathers have scoped out
			foreach (Vector3Int adjacent in GetMovementOptions(currentPos)) {
				// first, check if its a specific obstacle
				if (obstacles.Contains(adjacent)) continue;

				// units can move through units of similar types, but not enemy types
				int distSoFar = (distance.ContainsKey(currentPos)) ? distance[currentPos] : 0;
				var updatedCost = distSoFar + Cost(currentPos, adjacent);
				
				if (!distance.ContainsKey(adjacent) || updatedCost < distance[adjacent]) {
					distance[adjacent] = updatedCost;
					cameFrom[adjacent] = currentPos;
					pathQueue.Enqueue(distance[adjacent], adjacent);
				}
			}
		}
		
		// if we found the target, recount the path to get there
		if (foundTarget) {
			newPath.end = targetPosition; // space just outside of the target
			
			// init value only
			Vector3Int progenitor = targetPosition;
			
			while (progenitor != newPath.start) {
				var newProgenitor = cameFrom[progenitor];
				
				// build the path in reverse, aka next steps (including target)
				newPath.path[newProgenitor] = progenitor;
				progenitor = newProgenitor;
			}
		} else {
			// we didn't find a valid target/cost was too high. Stay put
			newPath.start = startPosition;
			newPath.end   = startPosition;
			newPath.path[startPosition] = startPosition;
		}

		return newPath;
	}

	public static MovingObjectPath GetAnyPathTo(Vector3Int startPosition, Vector3Int targetPosition) {
		return MovingObjectPath.GetPathTo(startPosition, targetPosition, new HashSet<Vector3Int>());
	}

	public static MovingObjectPath GetPathFromField(Vector3Int targetPosition, FlowField ffield) {
		IEnumerable<Vector3Int> GetFieldOptions(Vector3Int pos) {
			List<Vector3Int> options = new List<Vector3Int>() {
				pos + Vector3Int.up,
				pos + Vector3Int.right,
				pos + Vector3Int.down,
				pos + Vector3Int.left
			};
			foreach (Vector3Int opt in options) {
				if (ffield.field.ContainsKey(opt)) yield return opt;
			}
		}

		MovingObjectPath newPath = new MovingObjectPath(ffield.origin);
		newPath.start = ffield.origin;
		newPath.end   = targetPosition;
		
		// init position
		Vector3Int currentPos = targetPosition;
		
		PriorityQueue<Vector3Int> pathQueue = new PriorityQueue<Vector3Int>();
		pathQueue.Enqueue(0, currentPos);
		
		// BFS search here
		while (pathQueue.Count != 0)  {
			currentPos = pathQueue.Dequeue();
			
			// found the target, now recount the path
			if (currentPos == ffield.origin) break;

			Vector3Int bestMove = currentPos;
			int bestMoveCost = ffield.field[currentPos];
			foreach (Vector3Int adjacent in GetFieldOptions(currentPos)) {			
				int cost = ffield.field[adjacent];

				if (cost < bestMoveCost) {
					bestMoveCost = cost;
					bestMove = adjacent;
				}
			}

			if (bestMove != currentPos) {
				pathQueue.Enqueue(bestMoveCost, bestMove);
				newPath.path[bestMove] = currentPos;
			}
		}

		return newPath;
	}
	
	private static int CalcPriority(Vector3Int src, Vector3Int dest) {
		return (int)Vector3Int.Distance(src, dest);
	}

	private static int Cost(Vector3Int src, Vector3Int dest) {
		// the way we have coded cost into WorldTile:
		// the number listed is the cost to enter said tile
		var destTile = GameManager.inst.GetActiveGrid().GetTileAt(dest);
		return destTile.cost;
	}
	
	private static int EdgeCost(Vector3Int dest) {
		var destTile = GameManager.inst.GetActiveGrid().GetTileAt(dest);
		if (destTile == null) return -1;
		return destTile.cost;
	}
	
	private static int TotalPathCost(Vector3Int src, Vector3Int dest, Dictionary<Vector3Int, Vector3Int> cameFrom) {
		// for now, assume all keys are present
		Vector3Int progenitor = dest;
		int totalCost = 0;
		
		// build the path in reverse, aka next steps (including target)
		while (progenitor != src) {
			var progTile = GameManager.inst.GetActiveGrid().GetTileAt(progenitor);
			if (progTile == null) return -1;
			
			totalCost += progTile.cost;
			progenitor = cameFrom[progenitor];
		}
		return totalCost;
	}
	
	// this will disallow all movement through occupants, other than a specified template <T>
	private static List<Vector3Int> GetMovementOptions(Vector3Int fromPosition) {
		// since we call TakePhaseAction serially...
		// we don't need to know if an Enemy WILL move into a spot.
		// if they had higher priority, they will have already moved into it	
		// also, the conversion to HashSet, and the conversion back, is not worth it to remove from a list of 4 spaces max
		List<Vector3Int> moveOptions = new List<Vector3Int>();
		
		var grid = GameManager.inst.GetActiveGrid();
		foreach (Vector3Int pos in GameManager.inst.GetActiveGrid().GetNeighbors(fromPosition)) {
			if (grid.IsInBounds(pos)) {
				moveOptions.Add(pos);
			}
		}
		return moveOptions;
	}
	
	private void CalcStartEnd() {		
		HashSet<Vector3Int> keys = new HashSet<Vector3Int>(path.Keys);
		HashSet<Vector3Int> vals = new HashSet<Vector3Int>(path.Values);
		keys.SymmetricExceptWith(vals);
		
		foreach (Vector3Int either in keys) {
			if (path.ContainsKey(either)) _start = either;
			if (vals.Contains(either)) _end = either;
		}
	}
}
