﻿using System.Collections;
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
	public Water(){}
	public Water(Vector3Int pos) {
		position = pos;
	}
}