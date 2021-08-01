﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

public class DeepWater : Water
{
	public DeepWater(Vector3Int pos) {
		position = pos;
	}

	public override TacticsTile tacticsTile {
		get {
			if (_tacticsTile == null) {
				_tacticsTile = ScriptableObject.CreateInstance<ForestTacticsTile>();
			}
			return _tacticsTile;
		}
	}
}