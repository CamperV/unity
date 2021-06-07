using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

public class DeepForest : Forest, IEnemyArmySpawner
{
	public override int occlusion { get => FieldOfView.maxVisibility - 1; }
	public override int tickCost { get => (int)(Constants.standardTickCost * 2.5f); }
	
	public DeepForest(Vector3Int pos) {
		position = pos;
	}

	// IEnemyArmySpawner definitions
	public override Dictionary<float, string> spawnRates {
		get => new Dictionary<float, string>{
        	[5.00f] = "BanditArmy"
    	};
	}
}