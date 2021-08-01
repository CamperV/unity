using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

public class EmptyTerrain : Terrain
{
    public override TacticsTile tacticsTile {
		get {
			if (_tacticsTile == null) {
				_tacticsTile = ScriptableObject.CreateInstance<GrassTacticsTile>();
			}
			return _tacticsTile;
		}
	}
}