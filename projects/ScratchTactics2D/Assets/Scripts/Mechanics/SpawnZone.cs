using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Extensions;

public class SpawnZone : GridAlignedZone
{
    public SpawnZone(Vector3 _pivot, int _width, int _height, int _depth, TacticsGrid _grid) {
        pivot = _pivot;
        grid = _grid;

        // estimate the BL, and extrapolate the TR
        Vector3Int convertedPivot = grid.Real2GridPos(pivot - new Vector3(0, _grid.GetComponent<Grid>().cellSize.y/2.0f, 0));
       
        min = convertedPivot - new Vector3Int((int)(_width/2.0f), (int)(_height/2.0f), (int)(_depth/2.0f));
        max = min + new Vector3Int(_width, _height, _depth);

        positions = new HashSet<Vector3Int>();
        for (int x=min.x; x <= max.x; x++) {
            for (int y=min.y; y <= max.y; y++) {
                for (int z=min.z; z <= max.z; z++) {
                    Vector3Int v = new Vector3Int(x, y, z);
                    if (grid.IsInBounds(v) && grid.VacantAt(v)) positions.Add(v);
                }
            }
        }
    }

    public override void RecalculateCoverage() {
        int _width = width;
        int _height = height;
        int _depth = depth;

        // these will update .width, etc
        Vector3Int convertedPivot = grid.Real2GridPos(pivot - new Vector3(0, grid.GetComponent<Grid>().cellSize.y/2.0f, 0));
        min = convertedPivot - new Vector3Int((int)(_width/2.0f), (int)(_height/2.0f), (int)(_depth/2.0f));
        max = min + new Vector3Int(_width, _height, _depth);

        positions.Clear();
        for (int x=min.x; x <= max.x; x++) {
            for (int y=min.y; y <= max.y; y++) {
                for (int z=min.z; z <= max.z; z++) {
                    Vector3Int v = new Vector3Int(x, y, z);
                    if (grid.IsInBounds(v) && grid.VacantAt(v)) positions.Add(v);
                }
            }
        }       
    }
}
