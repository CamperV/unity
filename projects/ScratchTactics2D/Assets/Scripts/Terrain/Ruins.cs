using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

public class Ruins : Terrain
{
	public Ruins(Vector3Int pos) {
		position = pos;
	}
}