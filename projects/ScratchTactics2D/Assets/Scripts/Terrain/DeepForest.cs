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
	public override string tag { get => "Forest"; }

	public override int occlusion { get => 3; }
	public override int tickCost { get => (int)(Constants.standardTickCost * 2.5f); }
	
	public DeepForest(Vector3Int pos) {
		position = pos;
	}

	// IEnemyArmySpawner definitions
	public override Dictionary<float, string> spawnRates {
		get => new Dictionary<float, string>{
        	[4.00f] = "BanditArmy",
			[1.00f] = "BerserkerArmy"
    	};
	}
}