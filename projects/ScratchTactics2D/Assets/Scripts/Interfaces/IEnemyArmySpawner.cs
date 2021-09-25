using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public interface IEnemyArmySpawner
{
	// 0 - 99, determines how likely an enemy is to spawn at the start
	// each float maps to a EnemyArmy tag, which is dynamically loaded
	Dictionary<float, string> spawnRates { get; }

	// this will roll against its own tables, and spawn directly into the receivingController
	bool AttemptToSpawnArmy(Controller receivingController);
}