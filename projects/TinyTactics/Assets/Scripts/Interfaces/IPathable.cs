using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public interface IPathable<T> where T : struct
{
	IEnumerable<T> GetNeighbors(T v);
	int EdgeCost(T src, T dest);
}