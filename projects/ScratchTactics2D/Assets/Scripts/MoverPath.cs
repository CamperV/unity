using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverPath
{
	private Vector3Int _start;
	private Vector3Int _end;
	
	public Dictionary<Vector3Int, Vector3Int> path = new Dictionary<Vector3Int, Vector3Int>();
	
	public Vector3Int start {
		get { return _start; }
		set { _start = value; }
	}
	public Vector3Int end {
		get { return _end; }
		set { _end = value; }
	}
	
	public MoverPath() {}
	
	public void Clear() {
		path.Clear();
		start = new Vector3Int(-1, -1, -1);
		end   = new Vector3Int(-1, -1, -1);
	}
	
	public Vector3Int Next(Vector3Int position) {
		return path[position];
	}
	
	public void CalcStartEnd() {		
		HashSet<Vector3Int> keys = new HashSet<Vector3Int>(path.Keys);
		HashSet<Vector3Int> vals = new HashSet<Vector3Int>(path.Values);
		keys.SymmetricExceptWith(vals);
		
		foreach (Vector3Int either in keys) {
			if (path.ContainsKey(either)) _start = either;
			if (vals.Contains(either)) _end = either;
		}
	}
	
}
