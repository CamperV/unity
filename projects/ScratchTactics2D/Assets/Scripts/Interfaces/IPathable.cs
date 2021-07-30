using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public interface IPathable
{
	IEnumerable<Vector3Int> GetNeighbors(Vector3Int v);
	int EdgeCost(Vector3Int src, Vector3Int dest);
}