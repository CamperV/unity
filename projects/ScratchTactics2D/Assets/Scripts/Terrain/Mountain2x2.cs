﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

public class Mountain2x2 : Mountain
{
	public override int altitude { get => 4; }

	public Mountain2x2(Vector3Int pos) {
		position = pos;
		_tileRefPosition = pos;
	}
	public Mountain2x2(Vector3Int pos, Vector3Int tileRefPos) {
		position = pos;
		_tileRefPosition = tileRefPos;
	}
}