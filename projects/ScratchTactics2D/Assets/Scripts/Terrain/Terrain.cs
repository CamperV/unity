﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

public abstract class Terrain
{
	public Vector3Int position;
	public virtual int occlusion { get => 0; }
	public virtual Vector2Int battleGridSize { get => new Vector2Int(8, 8); }
	public virtual int tickCost { get => Constants.standardTickCost; }

	private TacticsTile _tacticsTile;
	public virtual TacticsTile tacticsTile {
		get {
			if (_tacticsTile == null) {
				_tacticsTile = ScriptableObject.CreateInstance<GrassIsoTile>();
			}
			return _tacticsTile;
		}
	}

	// IEnemyArmySpawner definitions
	public Controller receivingController { get => GameManager.inst.enemyController; }
	public virtual Dictionary<float, string> spawnRates {
		get => new Dictionary<float, string>{
			[float.MinValue] = "n/a"
		};
	}

	public bool AttemptToSpawnArmy() {
		float rng = Random.Range(0f, 100f);

		string armyTagToSpawn = null;
	    foreach (KeyValuePair<float, string> spawnPair in spawnRates.OrderBy(p => p.Key)) {
            if (rng <= spawnPair.Key) armyTagToSpawn = spawnPair.Value;
        }

		// unsuccessful, did not spawn an army
		if (armyTagToSpawn == null) return false;

		// otherwise:
		EnemyArmy armyPrefab = EnemyArmySpawner.LoadArmyByTag(armyTagToSpawn);
		EnemyArmy army = Army.Spawn<EnemyArmy>(armyPrefab, position);

		// this will update the ID value
		receivingController.Register(army);
		return true;
	}
}