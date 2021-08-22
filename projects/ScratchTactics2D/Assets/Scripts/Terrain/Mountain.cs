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
	protected Vector3Int _tileRefPosition;
	public override Vector3Int tileRefPosition { get => _tileRefPosition; }

	public override int occlusion { get => FieldOfView.maxVisibility; }
	public override int altitude { get => 2; }
	public override int tickCost { get => Constants.standardTickCost * 5; }

	public Mountain(){}
	public Mountain(Vector3Int pos) {
		position = pos;
		_tileRefPosition = pos;
	}
	public Mountain(Vector3Int pos, Vector3Int tileRefPos) {
		position = pos;
		_tileRefPosition = tileRefPos;
	}
}