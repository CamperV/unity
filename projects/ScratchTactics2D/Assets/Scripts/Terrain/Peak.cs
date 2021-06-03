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
	public override int occlusion { get => FieldOfView.maxVisibility; }
	
	public Peak(Vector3Int pos) {
		position = pos;
	}
}