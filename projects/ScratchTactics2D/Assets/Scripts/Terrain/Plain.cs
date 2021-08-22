﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

public class Plain : Terrain
{
	public override int tickCost { get => (int)(Constants.standardTickCost * 1.25f); }
	
    public Plain(Vector3Int pos) {
		position = pos;
	}
}