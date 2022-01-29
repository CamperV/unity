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
	
	public T Start => path.First.Value;
	public T End => path.Last.Value;
	public int Count => path.Count;
	
	public void AddFirst(T v) => path.AddFirst(v);
	public void AddLast(T v) => path.AddLast(v);
	public void Clear() => path.Clear();

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

	public static Path<T> MaskWithinRange<T>(Path<T> inputPath, FlowField<T> flowField) where T : struct {
		Path<T> newPath = new Path<T>();

		foreach (T currentPos in inputPath.Unwind()) {
			if (flowField.field.ContainsKey(currentPos)) {
				newPath.AddLast(currentPos);
			}
		}

		return newPath;
	}
}
