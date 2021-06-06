using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Path
{
	public Path() {}

	// YES, I'm aware of how many more iterations LL can have when compared to dictionaries
	// however, I need a convenient way to store Next/Previous without
	// also, Paths will almost only ever be used to iterate straight through, with very few Finds
	// it also greatly simplifies the utility functions I have to write
	protected LinkedList<Vector3Int> path = new LinkedList<Vector3Int>();
	
	public Vector3Int start { get => path.First.Value; }
	public Vector3Int end { get => path.Last.Value; }
	
	public void AddFirst(Vector3Int v) {
		path.AddFirst(v);
	}

	public void Clear() {
		path.Clear();
	}
	
	public Vector3Int GetNext(Vector3Int position) {
		return path.Find(position).Next.Value;
	}

	public Vector3Int GetPrevious(Vector3Int position) {
		return path.Find(position).Previous.Value;
	}
	
	public IEnumerable<Vector3Int> Unwind(int slice = 0) {
		LinkedListNode<Vector3Int> position = path.First;
		do {
			position = position.Next;
			
			// skip a certain number of tiles when unwinding
			if (slice > 0) {
				slice--;
				continue;
			}
			yield return position.Value;
		} while (position != path.Last);
	}

	public bool Contains(Vector3Int v) {
		return path.Contains(v);
	}
	
	public bool IsEmpty() {
		return path.Count == 0 || path == null;
	}
		
	public void Clip(MoveRange mRange) {
		// unwind until out of bounds, then start whackin'
		List<Vector3Int> forRemoval = new List<Vector3Int>();
		foreach (var pos in path) {
			if (!mRange.field.ContainsKey( pos )) {
				forRemoval.Add(pos);
			}
		}

		// use two loops to avoid modifying path during iteration
		foreach (var fr in forRemoval) {
			path.Remove(fr);
		}
	}

	public IEnumerable<Vector3Int> Serially() {
		LinkedListNode<Vector3Int> position = path.First;
		LinkedListNode<Vector3Int> prevPosition = position;
		do {
			prevPosition = position;
			position = position.Next;
			yield return position.Value - prevPosition.Value;
		} while (position != path.Last);
	}
}
