using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

public class TerrainPatternShape : HashSet<Vector3Int>
{
    // All shapes must be defined by an origin, see "Bottom Left"
    public static TerrainPatternShape BottomLeftSquare { get => new TerrainPatternShape{ Vector3Int.zero, Vector3Int.right, Vector3Int.up, Vector3Int.up+Vector3Int.right }; }

    public static TerrainPatternShape FromList(List<Vector3Int> list) {
        var retVal = new TerrainPatternShape();
        foreach (var v in list) { retVal.Add(v); }
        return retVal;
    }

    public bool Matches(TerrainPatternShape other) {
        return this.SetEquals(other);
    }
}