using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

public class TerrainPatternShape : TerrainPattern
{
    // All shapes must be defined by an origin, see "Bottom Left"
    public static TerrainPatternShape BottomLeftSquare { get => new TerrainPatternShape{ Vector3Int.zero, Vector3Int.right, Vector3Int.up, Vector3Int.up+Vector3Int.right }; }
}