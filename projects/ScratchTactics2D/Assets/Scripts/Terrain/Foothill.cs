﻿using System.Collections;
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
	public override int occlusion { get => 1; }

	public Foothill(Vector3Int pos) {
		position = pos;
	}
}