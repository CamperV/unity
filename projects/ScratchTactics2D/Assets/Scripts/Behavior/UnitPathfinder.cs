using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class UnitPathfinder : ObstaclePathfinder
{
	public UnitPathfinder(HashSet<Vector3Int> _obstacles) : base(_obstacles) {
		pathableSurface = GameManager.inst.tacticsManager.GetActiveGrid();
	}
	public UnitPathfinder() : base() {
		pathableSurface = GameManager.inst.tacticsManager.GetActiveGrid();
	}
}
