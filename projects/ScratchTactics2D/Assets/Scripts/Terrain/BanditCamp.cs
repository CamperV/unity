﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

public class BanditCamp : Terrain, IEnemyArmySpawner
{
	public BanditCamp(Vector3Int pos) {
		position = pos;
	}

	public override TacticsTile tacticsTile {
		get {
			if (_tacticsTile == null) {
				_tacticsTile = ScriptableObject.CreateInstance<GrassTacticsTile>();
			}
			return _tacticsTile;
		}
	}

	// IEnemyArmySpawner definitions
	public override Dictionary<float, string> spawnRates {
		get => new Dictionary<float, string>{
        	[100.00f] = "BanditArmy"
    	};
	}
}