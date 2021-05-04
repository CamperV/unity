using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

public class Village : Terrain
{
	public Vector3Int position;

	public Village(Vector3Int pos) {
		position = pos;
	}

	public override void Apply(WorldGrid grid) {}
}