using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

public class Water : Terrain
{	
	public override int tickCost { get => Constants.standardTickCost * 5; }

	public Water(){}
	public Water(Vector3Int pos) {
		position = pos;
	}

    public override TacticsTile tacticsTile {
		get {
			if (_tacticsTile == null) {
				_tacticsTile = ScriptableObject.CreateInstance<WaterIsoTile>();
			}
			return _tacticsTile;
		}
	}
}