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
}