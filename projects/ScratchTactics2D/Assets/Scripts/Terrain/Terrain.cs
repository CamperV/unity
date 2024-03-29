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
	public virtual string tag { get => this.GetType().Name; }

	public virtual Vector3Int tileRefPosition { get => position; }
	public virtual int occlusion { get => 0; }
	public virtual int altitude { get => 0; }
	public virtual int tickCost { get => Constants.standardTickCost; }
	public virtual int foodCost { get => 1; }	// relevant to PlayerArmy only
	protected bool appliedFlag = false;

	// IEnemyArmySpawner definitions
	public virtual Dictionary<float, string> spawnRates {
		get => new Dictionary<float, string>{
			[float.MinValue] = "n/a"
		};
	}

	public bool AttemptToSpawnArmy(Controller receivingController) {
		//			0 - 199
		float rng = Random.Range(0, 200) / 2f;
		float cumValue = 0f;	// lmao

		string armyTagToSpawn = null;
	    foreach (KeyValuePair<float, string> spawnPair in spawnRates.OrderBy(p => p.Key)) {
			cumValue += spawnPair.Key;

            if (cumValue >= rng) {
				armyTagToSpawn = spawnPair.Value;
				break;
			}
        }

		// unsuccessful, did not spawn an army
		if (armyTagToSpawn == null) return false;

		// otherwise:
		EnemyArmy armyPrefab = EnemyArmySpawner.LoadArmyByTag("Armies/" + armyTagToSpawn);
		EnemyArmy army = Army.Spawn<EnemyArmy>(armyPrefab, position);

		// this will update the ID value
		receivingController.Register(army);
		return true;
	}

	public virtual void ApplyTerrainEffect(Army enteringArmy) {
		if (!appliedFlag) appliedFlag = true;
	}
}