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
    
    // o o
    // x o
    public static TerrainPatternShape BottomLeftSquare { get => new TerrainPatternShape{ Vector3Int.zero, Vector3Int.right, Vector3Int.up, Vector3Int.up+Vector3Int.right }; }
    
    //   o
    // o x o
    //   o
    public static TerrainPatternShape CenterPlus { get => new TerrainPatternShape{ Vector3Int.zero, Vector3Int.right, Vector3Int.up, Vector3Int.left, Vector3Int.down }; }
    
    //   o
    // o   o
    //   o
    public static TerrainPatternShape NoCenterPlus { get => new TerrainPatternShape{ Vector3Int.right, Vector3Int.up, Vector3Int.left, Vector3Int.down }; }
}