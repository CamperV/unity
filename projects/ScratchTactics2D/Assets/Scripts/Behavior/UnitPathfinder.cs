using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class UnitPathfinder : ObstaclePathfinder
{
	public UnitPathfinder(HashSet<Vector3Int> _obstacles) : base(_obstacles) {
		pathableSurface = Battle.active.grid;
	}
	public UnitPathfinder() : base() {
		pathableSurface = Battle.active.grid;
	}
}
