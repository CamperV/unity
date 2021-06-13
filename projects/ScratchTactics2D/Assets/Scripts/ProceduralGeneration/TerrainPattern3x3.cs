using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

public class TerrainPattern3x3 : TerrainPattern
{
    // ORDER MATTERS HERE - see the RoadWorldTile sprite ordering
    public static TerrainPattern3x3 North { get => new TerrainPattern3x3{ Vector3Int.up }; }
    public static TerrainPattern3x3 South { get => new TerrainPattern3x3{ Vector3Int.down }; }
    public static TerrainPattern3x3 East { get => new TerrainPattern3x3{ Vector3Int.right }; }
    public static TerrainPattern3x3 West { get => new TerrainPattern3x3{ Vector3Int.left }; }
    //
    public static TerrainPattern3x3 EastWest { get => new TerrainPattern3x3{ Vector3Int.left, Vector3Int.right }; }
    public static TerrainPattern3x3 NorthWest { get => new TerrainPattern3x3{ Vector3Int.up, Vector3Int.left }; }
    public static TerrainPattern3x3 NorthEast { get => new TerrainPattern3x3{ Vector3Int.up, Vector3Int.right }; }
    public static TerrainPattern3x3 SouthEast { get => new TerrainPattern3x3{ Vector3Int.down, Vector3Int.right }; }
    public static TerrainPattern3x3 SouthWest { get => new TerrainPattern3x3{ Vector3Int.down, Vector3Int.left }; }
    public static TerrainPattern3x3 NorthSouth { get => new TerrainPattern3x3{ Vector3Int.up, Vector3Int.down }; }
    public static TerrainPattern3x3 NorthSouthEast { get => new TerrainPattern3x3{ Vector3Int.up, Vector3Int.down, Vector3Int.right }; }
    public static TerrainPattern3x3 NorthSouthWest { get => new TerrainPattern3x3{ Vector3Int.up, Vector3Int.down, Vector3Int.left }; }
    public static TerrainPattern3x3 NorthEastWest { get => new TerrainPattern3x3{ Vector3Int.up, Vector3Int.left, Vector3Int.right }; }
    public static TerrainPattern3x3 SouthEastWest { get => new TerrainPattern3x3{ Vector3Int.down, Vector3Int.left, Vector3Int.right }; }
    public static TerrainPattern3x3 NorthSouthEastWest { get => new TerrainPattern3x3{ Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right }; }
    
    public T GetPatternTile<T>() where T : WorldTile {
        int match = MatchStandardPattern();
        Debug.Assert(match > -1);

        MethodInfo methodInfo = typeof(T).GetMethod("GetTileWithSprite");
        return (T)methodInfo.Invoke(null, new object[] { Mathf.Max(0, match) });
	}

    private int MatchStandardPattern() {
        List<TerrainPattern3x3> toMatchAgainst = new List<TerrainPattern3x3>() {
            TerrainPattern3x3.EastWest,
            TerrainPattern3x3.NorthWest,
            TerrainPattern3x3.NorthEast,
            TerrainPattern3x3.SouthEast,
            TerrainPattern3x3.SouthWest,
            TerrainPattern3x3.NorthSouth,
            TerrainPattern3x3.NorthSouthEast,
            TerrainPattern3x3.NorthSouthWest,
            TerrainPattern3x3.NorthEastWest,
            TerrainPattern3x3.SouthEastWest,
            TerrainPattern3x3.NorthSouthEastWest,
            // new
            TerrainPattern3x3.North,
            TerrainPattern3x3.South,
            TerrainPattern3x3.East,
            TerrainPattern3x3.West
        };
        for(int i = 0; i < toMatchAgainst.Count; i++) {
            if (this.SetEquals(toMatchAgainst[i])) return i;
        }
        return -1;
    }
}