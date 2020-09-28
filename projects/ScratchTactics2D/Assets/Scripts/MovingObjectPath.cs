using System.Collections;
using System.Collections.Generic;
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
		//int ind = path.IndexOf(position);
		//if (ind == path.Count-1) return end;
		//return path[ind+1];
	}
	
	public Vector3Int PopNext(Vector3Int position) {
		// hate that we have to double-search here, but hey, its the API
		// no available Remove(key, out val) override in Unity
		Vector3Int retval = path[position];
		path.Remove(position);
		GameManager.inst.worldGrid.ResetOverlayAt(position, pathOverlayTile.level);
		return retval;
	}
	
	public void Consume(Vector3Int position) {
		path.Remove(position);
		//GameManager.inst.worldGrid.ResetOverlayAt(position, pathOverlayTile.level);
		GameManager.inst.worldGrid.ClearTilesOnLevel(pathOverlayTile.level);
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
			GameManager.inst.worldGrid.ResetOverlayAt(tile, pathOverlayTile.level);
		}
		GameManager.inst.worldGrid.ResetOverlayAt(end, endpointOverlayTile.level);
	}	
		
	// AI pathfinding
	// storing this is a hashmap also helps for quickly assessing what squares are available
	// T is the type you expect to path towards, and don't mind a collision against
	public static MovingObjectPath GetPathTo(Vector3Int startPosition, Vector3Int targetPosition, int costHeuristic) {
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
				int distSoFar = (distance.ContainsKey(currentPos)) ? distance[currentPos] : 0;
				var updatedCost = distSoFar + Cost(currentPos, adjacent);
				
				if (updatedCost > costHeuristic)
					continue;
				
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
	
	private static int CalcPriority(Vector3Int src, Vector3Int dest) {
		return (int)Vector3Int.Distance(src, dest);
	}
	
	private static int EdgeCost(Vector3Int dest) {
		var destTile = GameManager.inst.worldGrid.GetWorldTileAt(dest);
		if (destTile == null) return -1;
		return destTile.cost;
	}
	
	private static int Cost(Vector3Int src, Vector3Int dest) {
		// the way we have coded cost into WorldTile:
		// the number listed is the cost to enter said tile
		//var srcTile  = GameManager.inst.worldGrid.GetWorldTileAt(src);
		var destTile = GameManager.inst.worldGrid.GetWorldTileAt(dest);
		return destTile.cost;
	}
	
	private static int TotalPathCost(Vector3Int src, Vector3Int dest, Dictionary<Vector3Int, Vector3Int> cameFrom) {
		// for now, assume all keys are present
		Vector3Int progenitor = dest;
		int totalCost = 0;
		
		// build the path in reverse, aka next steps (including target)
		while (progenitor != src) {
			var progTile = GameManager.inst.worldGrid.GetWorldTileAt(progenitor);
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
		
		foreach (Vector3Int pos in GameManager.inst.worldGrid.GetNeighbors(fromPosition)) {
			if (!GameManager.inst.worldGrid.IsInBounds(pos)) {
				continue;
			}
			var occupant = GameManager.inst.worldGrid.OccupantAt(pos);
			
			// either check the tag or type of occupant
			// if occupant is null, short-circuit and add moveOption
			// if it is occupied, but is a Player, still works
			if (occupant != null) {
				continue;
			}
			moveOptions.Add(pos);
		}
		return moveOptions;
	}
	
	//
	//
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
