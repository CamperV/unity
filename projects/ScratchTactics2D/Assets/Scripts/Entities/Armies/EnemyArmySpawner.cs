using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

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