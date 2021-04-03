using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Extensions;

public class Zone
{
    protected HashSet<Vector3Int> positions;
    public Vector3Int min;
    public Vector3Int max;

    public int width { get => max.x - min.x; }
    public int height { get => max.y - min.y; }
    public int depth { get => max.z - min.z; }
    public int Count { get => positions.Count; }

    // for some reason... BoundsInt isn't behaving like I'd like
    // time for tried and true (but boring) for loops...
    public Zone() {}
    public Zone(Vector3Int bl, Vector3Int tr) {

        min = new Vector3Int(
            (bl.x < tr.x) ? bl.x : tr.x,
            (bl.y < tr.y) ? bl.y : tr.y,
            (bl.z < tr.z) ? bl.z : tr.z
        );
        max = new Vector3Int(
            (bl.x > tr.x) ? bl.x : tr.x,
            (bl.y > tr.y) ? bl.y : tr.y,
            (bl.z > tr.z) ? bl.z : tr.z
        );
        
        positions = new HashSet<Vector3Int>();
        for (int x=min.x; x <= max.x; x++) {
            for (int y=min.y; y <= max.y; y++) {
                for (int z=min.z; z <= max.z; z++) {
                    positions.Add(new Vector3Int(x, y, z));
                }
            }
        }
    }

    public static Zone WithinGrid(GameGrid grid, Vector3Int bl, Vector3Int tr) {
        Zone zone = new Zone(bl, tr);
        zone.positions.RemoveWhere(v => !grid.IsInBounds(v));
        return zone;
    }

    public static Zone VacantWithinGrid(GameGrid grid, Vector3Int bl, Vector3Int tr) {
        Zone zone = new Zone(bl, tr);
        zone.positions.RemoveWhere(v => !grid.IsInBounds(v) || grid.OccupantAt(v) != null);
        return zone;
    }

    public List<Vector3Int> GetPositions() {
        return positions.ToList();
    }
}
