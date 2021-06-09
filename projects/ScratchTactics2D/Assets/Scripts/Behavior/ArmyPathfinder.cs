using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class ArmyPathfinder : ObstaclePathfinder
{
	HashSet<Vector3Int> interactable;

	public ArmyPathfinder() : base() {
		pathableSurface = GameManager.inst.overworld;
	}
	public ArmyPathfinder(HashSet<Vector3Int> _obstacles) : base(_obstacles) {
		pathableSurface = GameManager.inst.overworld;
	}
	public ArmyPathfinder(HashSet<Vector3Int> _obstacles, int tickThreshold) : base(_obstacles) {
		pathableSurface = GameManager.inst.overworld;

		// update your obstacles based on what you can't move through anyway
		foreach (var t in GameManager.inst.overworld.Terrain.Where(it => it.tickCost > tickThreshold)) {
			obstacles.Add(t.position);
		}
	}

	public OverworldPath NullableBFS(Vector3Int startPosition, Vector3Int targetPosition) {
		return BFS<OverworldPath>(startPosition, targetPosition);
	}
}
