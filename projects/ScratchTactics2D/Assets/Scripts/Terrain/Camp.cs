﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

public class Camp : Terrain, IEnemyArmySpawner
{
	public Camp(Vector3Int pos) {
		position = pos;
	}

	public override void ApplyTerrainEffect(Army enteringArmy) {
		if (!appliedFlag) {
			if (enteringArmy.armyTag == "PlayerArmy") {
				GlobalPlayerState.UpdateFood(+15);
			}
			
			appliedFlag = true;
		}
	}

	// IEnemyArmySpawner definitions
	public override Dictionary<float, string> spawnRates {
		get => new Dictionary<float, string>{
			[2.50f] = "BanditArmy",
        	[2.50f] = "BerserkerArmy"			
    	};
	}
}