using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Extensions;

public class Zone
{
    private HashSet<Vector3Int> allPositions;

    // for some reason... BoundsInt isn't behaving like I'd like
    // time for tried and true (but boring) for loops...
    public Zone(Vector3Int bl, Vector3Int tr) {
        allPositions = new HashSet<Vector3Int>();

        var min  = new Vector3Int(
            (bl.x < tr.x) ? bl.x : tr.x,
            (bl.y < tr.y) ? bl.y : tr.y,
            (bl.z < tr.z) ? bl.z : tr.z
        );
        var max  = new Vector3Int(
            (bl.x > tr.x) ? bl.x : tr.x,
            (bl.y > tr.y) ? bl.y : tr.y,
            (bl.z > tr.z) ? bl.z : tr.z
        );
        
        for (int x=min.x; x <= max.x; x++) {
            for (int y=min.y; y <= max.y; y++) {
                for (int z=min.z; z <= max.z; z++) {
                    allPositions.Add(new Vector3Int(x, y, z));
                }
            }
        }
    }

    public static Zone WithinGrid(GameGrid grid, Vector3Int bl, Vector3Int tr) {
        Zone zone = new Zone(bl, tr);
        zone.allPositions.RemoveWhere((Vector3Int v) => {
            return !grid.IsInBounds(v);
        });
        return zone;
    }

    public int Count { get => allPositions.Count; }

    public List<Vector3Int> GetPositions() {
        return allPositions.ToList();
    }

}
