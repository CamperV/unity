using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

public class Mountain : Terrain
{
	public override int occlusion { get => FieldOfView.maxVisibility; }
	public override int tickCost { get => Constants.standardTickCost * 20; }

	public Mountain(){}
	public Mountain(Vector3Int pos) {
		position = pos;
	}
}