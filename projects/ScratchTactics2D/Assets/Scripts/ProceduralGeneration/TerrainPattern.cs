using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

public class TerrainPattern : HashSet<Vector3Int>
{
    private int _width = -1;
    public int width {
        get {
            if (_width == -1) {
                _width = this.Max(it => it.x) - this.Min(it => it.x) + 1; 
            }
            return _width;
        }
    }

    private int _height = -1;
    public int height {
        get {
            if (_height == -1) {
                _height = this.Max(it => it.y) - this.Min(it => it.y) + 1;
            }
            return _height;
        }
    }

    public static TerrainPattern FromList(List<Vector3Int> list) {
        var retVal = new TerrainPattern();
        list.ForEach( it => retVal.Add(it) );
        return retVal;
    }
    
    public bool Matches(TerrainPattern other) {
        return this.SetEquals(other);
    }

    public IEnumerable<Vector3Int> YieldPattern(Vector3Int origin) {
        foreach(var v in this) {
            yield return origin + v;
        }
    }
    
    public IEnumerable<Vector3Int> YieldPatternExcept(Vector3Int origin, Vector3Int except) {
        foreach(var v in this) {
            if (origin + v == except) continue;
            yield return origin + v;
        }
    }
}