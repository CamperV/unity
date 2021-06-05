using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ArmyPathfinder : ObstaclePathfinder
{
	public ArmyPathfinder(HashSet<Vector3Int> _obstacles) : base(_obstacles) {
		pathableSurface = GameManager.inst.overworld;
	}
	public ArmyPathfinder() : base() {
		pathableSurface = GameManager.inst.overworld;
	}
}
