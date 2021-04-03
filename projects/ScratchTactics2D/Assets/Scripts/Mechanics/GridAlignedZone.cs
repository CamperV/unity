using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Extensions;

public class GridAlignedZone : Zone
{
    public Vector3 pivot;
    public TacticsGrid grid;

    // a GA-Zone is type of zone that has a float pivot, but the positions within round to their best spots within a grid
    public GridAlignedZone() {}
    public GridAlignedZone(Vector3 _pivot, int _width, int _height, int _depth, TacticsGrid _grid) {
        pivot = _pivot;
        grid = _grid;

        // estimate the BL, and extrapolate the TR
        min = grid.Real2GridPos(pivot) - new Vector3Int((int)(_width/2.0f), (int)(_height/2.0f), (int)(_depth/2.0f));
        max = min + new Vector3Int(_width, _height, _depth);

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

	public void Display() {
		foreach (Vector3Int tilePos in positions) {
			grid.UnderlayAt(tilePos, Constants.zoneColorViolet);
		}
	}

	public void ClearDisplay() {
		foreach (Vector3Int tilePos in positions) {
			grid.ResetUnderlayAt(tilePos);
		}
	}

    public void RecalculateCoverage() {
        int _width = width;
        int _height = height;
        int _depth = depth;

        // these will update .width, etc
        min = grid.Real2GridPos(pivot) - new Vector3Int((int)(_width/2.0f), (int)(_height/2.0f), (int)(_depth/2.0f));
        max = min + new Vector3Int(_width, _height, _depth);

        positions.Clear();
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
