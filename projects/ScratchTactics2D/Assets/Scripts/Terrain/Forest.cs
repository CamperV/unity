using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

public class Forest : Terrain
{
	public override int occlusion { get => FieldOfView.maxVisibility - 1; }
	public Vector3Int position;

	public Forest(Vector3Int pos) {
		position = pos;
	}

	public override void Apply(WorldGrid grid) {}
}