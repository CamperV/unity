using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

interface IEnemyArmySpawner
{
	// 0 - 99, determines how likely an enemy is to spawn at the start
	// each float maps to a EnemyArmy tag, which is dynamically loaded
	Dictionary<float, string> spawnRates { get; }
	Controller receivingController { get; }

	// this will roll against its own tables, and spawn directly into the receivingController
	bool AttemptToSpawnArmy();
}

public static class EnemyArmySpawner
{
	private static Dictionary<string, EnemyArmy> armyPrefabs = new Dictionary<string, EnemyArmy>();

	public static EnemyArmy LoadArmyByTag(string tag) {
		EnemyArmy armyPrefab;
		if (armyPrefabs.ContainsKey(tag)) {
			armyPrefab = armyPrefabs[tag];
		} else {
			armyPrefab = Resources.Load<EnemyArmy>(tag);
			armyPrefabs.Add(tag, armyPrefab);
		}
		return armyPrefab;
	}
}