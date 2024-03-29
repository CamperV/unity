﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Path<T> where T : struct, IEquatable<T>
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

	public static Path<T> Copy(Path<T> copyFrom) {
		Path<T> newPath = new Path<T>();
		foreach(T t in copyFrom.Unwind()) {
			newPath.AddLast(t);
		}
		return newPath;
	}

	public static void Print(Path<T> path) {
		int position = 0;
		foreach(T t in path.Unwind()) {
			Debug.Log($"{position}: {t}");
			position++;
		}
	}

	public T GetNext(T position) {
		return path.Find(position).Next.Value;
	}

	public T GetPrevious(T position) {
		return path.Find(position).Previous.Value;
	}
	
	public IEnumerable<T> Unwind() {
		LinkedListNode<T> position = path.First;
		while (true) {
			yield return position.Value;

			if (position != path.Last) {
				position = position.Next;
			} else { break; }
		}
	}

	public IEnumerable<LinkedListNode<T>> UnwindNodes() {
		LinkedListNode<T> position = path.First;
		while (true) {
			yield return position;

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

	public static Path<T> MaskWithinRange(Path<T> inputPath, FlowField<T> flowField) {
		Path<T> newPath = new Path<T>();

		foreach (T currentPos in inputPath.Unwind()) {
			if (flowField.field.ContainsKey(currentPos)) {
				newPath.AddLast(currentPos);
			}
		}

		return newPath;
	}

	// these paths must come to us IN ORDER
	// ie, their Ends must overlap with another's Start
	// then we actually skip those instances
	public static Path<T> MergePaths(IEnumerable<Path<T>> paths) {
		Path<T> newPath = new Path<T>();

		foreach (Path<T> path in paths) {
			foreach (T currentPos in path.Unwind()) {
				if (newPath.Count > 0 && currentPos.Equals(newPath.End))
					continue;
				
				newPath.AddLast(currentPos);
			}
		}

		return newPath;
	}
}
