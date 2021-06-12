using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

public class Foothill : Terrain
{
	public override int altitude { get => 1; }
	public override int occlusion { get => 1; }
	public override int tickCost { get => (int)(Constants.standardTickCost * 1.5f); }

	public Foothill(Vector3Int pos) {
		position = pos;
	}
}