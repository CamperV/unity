using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

public abstract class Terrain
{
	public Vector3Int position;
	public virtual int occlusion { get => 0; }
	public virtual Vector2Int battleGridSize { get => new Vector2Int(8, 8); }
	
	protected TacticsTile _tacticsTile;
	public virtual TacticsTile tacticsTile {
		get {
			if (_tacticsTile == null) {
				_tacticsTile = ScriptableObject.CreateInstance<GrassIsoTile>();
			}
			return _tacticsTile;
		}
	}
}