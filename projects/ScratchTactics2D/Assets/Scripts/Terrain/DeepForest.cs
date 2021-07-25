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
	public override int tickCost { get => (int)(Constants.standardTickCost * 2.5f); }
	
	public DeepForest(Vector3Int pos) {
		position = pos;
	}

	public override TacticsTile tacticsTile {
		get {
			if (_tacticsTile == null) {
				_tacticsTile = ScriptableObject.CreateInstance<ForestIsoTile>();
			}
			return _tacticsTile;
		}
	}
}