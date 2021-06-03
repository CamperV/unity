using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

public class DeepForest : Forest
{
	public override int occlusion { get => FieldOfView.maxVisibility - 1; }
	
	public DeepForest(Vector3Int pos) {
		position = pos;
	}
}