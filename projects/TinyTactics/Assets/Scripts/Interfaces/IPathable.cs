using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public interface IPathable<T> where T : struct
{
	IEnumerable<T> GetNeighbors(T t);
	int BaseCost(T t);
}