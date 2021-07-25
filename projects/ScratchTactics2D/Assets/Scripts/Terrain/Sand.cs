using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

public class Sand : Terrain
{
	public override int tickCost { get => (int)(Constants.standardTickCost * 1.25f); }
	
	public Sand(Vector3Int pos) {
		position = pos;
	}

    public override TacticsTile tacticsTile {
		get {
			if (_tacticsTile == null) {
				_tacticsTile = ScriptableObject.CreateInstance<GrassIsoTile>();
			}
			return _tacticsTile;
		}
	}
}