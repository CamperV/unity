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
	public override int occlusion { get => FieldOfView.maxVisibility - 2; }
	public override int tickCost { get => Constants.standardTickCost * 2; }

	public Forest(){}
	public Forest(Vector3Int pos) {
		position = pos;
	}
}