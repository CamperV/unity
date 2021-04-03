using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Extensions;

public class BurstZone : GridAlignedZone
{
    public BurstZone(Vector3 _pivot, int radius, TacticsGrid _grid) {
        pivot = _pivot;
        grid = _grid;

        // estimate the BL, and extrapolate the TR
        min = grid.Real2GridPos(pivot) - new Vector3Int(radius, radius, radius);
        max = min + new Vector3Int(radius*2, radius*2, radius*2);

        positions = new HashSet<Vector3Int>();
        for (int x=min.x; x <= max.x; x++) {
            for (int y=min.y; y <= max.y; y++) {
                for (int z=min.z; z <= max.z; z++) {
                    Vector3Int v = new Vector3Int(x, y, z);
                    if (grid.IsInBounds(v)) positions.Add(v);
                }
            }
        }
    }
}
