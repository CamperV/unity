using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

public class Peak : Mountain
{
	public override int altitude { get => 3; }
	
	public Peak(){}
	public Peak(Vector3Int pos) {
		position = pos;
		_tileRefPosition = pos;
	}
	public Peak(Vector3Int pos, Vector3Int tileRefPos) {
		position = pos;
		_tileRefPosition = tileRefPos;
	}

    public override TacticsTile tacticsTile {
		get {
			if (_tacticsTile == null) {
				_tacticsTile = ScriptableObject.CreateInstance<MountainIsoTile>();
			}
			return _tacticsTile;
		}
	}
}