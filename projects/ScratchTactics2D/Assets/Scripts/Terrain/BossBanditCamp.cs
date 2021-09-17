using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

public class BossBanditCamp : Terrain
{
	public BossBanditCamp(Vector3Int pos) {
		position = pos;
	}

	public override void ApplyTerrainEffect(Army enteringArmy) {
		if (!appliedFlag) {
			if (enteringArmy.armyTag == "PlayerArmy") {
				Debug.Log($"Let's initiate that BossBattle!");
				Battle.CreateSpecialBattle((enteringArmy as PlayerArmy), this, Enum.Phase.player);
			}
			
			appliedFlag = true;
		}
	}
}