using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverPath
{
	private Vector3Int _start;
	private Vector3Int _end;
	private PathOverlayTile pathOverlayTile;
	
	public Dictionary<Vector3Int, Vector3Int> path = new Dictionary<Vector3Int, Vector3Int>();
	public Vector3Int start {
		get { return _start; }
		set { _start = value; }
	}
	public Vector3Int end {
		get { return _end; }
		set { _end = value; }
	}
	
	public MoverPath() {
		pathOverlayTile = ScriptableObject.CreateInstance<PathOverlayTile>() as PathOverlayTile;
	}	 
	
	public void Clear() {
		ResetDrawPath();
		path.Clear();
		start = new Vector3Int(-1, -1, -1);
		end   = new Vector3Int(-1, -1, -1);
	}
	
	public Vector3Int Next(Vector3Int position) {
		return path[position];
	}
	
	public Vector3Int PopNext(Vector3Int position) {
		// hate that we have to double-search here, but hey, its the API
		// no available Remove(key, out val) override in Unity
		Vector3Int retval = path[position];
		path.Remove(position);
		GameManager.inst.worldGrid.ResetOverlayAt(position, pathOverlayTile.level);
		return retval;
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
	}
	
	public void ResetDrawPath() {
		foreach (Vector3Int tile in path.Keys) {
			GameManager.inst.worldGrid.ResetOverlayAt(tile, pathOverlayTile.level);
		}
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
