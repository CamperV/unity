using System.Collections;
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

	// IEnemyArmySpawner definitions
	public Controller receivingController { get => GameManager.inst.enemyArmyController; }
	public virtual Dictionary<float, string> spawnRates {
		get => new Dictionary<float, string>{
			[float.MinValue] = "n/a"
		};
	}

	public bool AttemptToSpawnArmy() {
		// Unity's float!float is inclusive for both sides
		float rng = Random.Range(float.Epsilon, 100f);

		string armyTagToSpawn = null;
	    foreach (KeyValuePair<float, string> spawnPair in spawnRates.OrderBy(p => p.Key)) {
            if (rng <= spawnPair.Key) armyTagToSpawn = spawnPair.Value;
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
		// pass
	}
}