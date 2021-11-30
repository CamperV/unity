using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Path<T> where T : struct
{
	public Path() {}

	// YES, I'm aware of how many more iterations LL can have when compared to dictionaries
	// however, I need a convenient way to store Next/Previous without
	// also, Paths will almost only ever be used to iterate straight through, with very few Finds
	// it also greatly simplifies the utility functions I have to write
	protected LinkedList<T> path = new LinkedList<T>();
	
	public T start { get => path.First.Value; }
	public T end { get => path.Last.Value; }
	
	public void AddFirst(T v) {
		path.AddFirst(v);
	}

	public void Clear() {
		path.Clear();
	}
	
	public T GetNext(T position) {
		return path.Find(position).Next.Value;
	}

	public T GetPrevious(T position) {
		return path.Find(position).Previous.Value;
	}
	
	public IEnumerable<T> Unwind(int slice = 0) {
		LinkedListNode<T> position = path.First;
		while (true) {
			// skip a certain number of tiles when unwinding
			if (slice > 0) {
				slice--;
				continue;
			}
			yield return position.Value;

			if (position != path.Last) {
				position = position.Next;
			} else { break; }
		}
	}

	public bool Contains(T v) {
		return path.Contains(v);
	}
	
	public bool IsEmpty() {
		return path.Count == 0 || path == null;
	}
		
	// public void Clip(MoveRange mRange) {
	// 	// unwind until out of bounds, then start whackin'
	// 	List<T> forRemoval = new List<T>();
	// 	foreach (var pos in path) {
	// 		if (!mRange.field.ContainsKey( pos )) {
	// 			forRemoval.Add(pos);
	// 		}
	// 	}

	// 	// use two loops to avoid modifying path during iteration
	// 	foreach (var fr in forRemoval) {
	// 		path.Remove(fr);
	// 	}
	// }

	// public IEnumerable<T> Serially() {
	// 	LinkedListNode<T> position = path.First;
	// 	LinkedListNode<T> prevPosition = position;
	// 	do {
	// 		prevPosition = position;
	// 		position = position.Next;
	// 		yield return position.Value - prevPosition.Value;
	// 	} while (position != path.Last);
	// }
}
