using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Extensions;
using System.Linq;

public class FieldOfView
{
	public static int maxVisibility = 15;

	// this differs from a FlowField because it relies on line-of-site
	// as such, instead of pathfinding, we'll calculate the LoS for every involved tile
	public Vector3Int origin;
	public Dictionary<Vector3Int, int> field;

	public FieldOfView(Vector3Int _origin, int range, WorldGrid grid) {
		origin = _origin;
		field = new Dictionary<Vector3Int, int> {
			[origin] = 0
		};

		// reverse raycast
		foreach(Vector3Int v in origin.RadiateSquare(range)) {
			if (!grid.IsInBounds(v)) continue;
			
			// draw a line between this tile and the origin
			// get all tiles along the line, and count up their occlusion + total with the number of tiles covered
			// interpolate for atleast twice the range, b/c sampling theroem?
			int occlusion = MinimumRaycastCost(v, origin, grid);
			if (occlusion <= range) {
				field[v] = occlusion;
			}
		}
	}

	private int MinimumRaycastCost(Vector3Int src, Vector3Int dest, WorldGrid grid) {
		Vector3 realSrc = grid.Grid2RealPos(src);
		Vector3 realDest = grid.Grid2RealPos(dest);
		float buffer = grid.GetComponent<Grid>().cellSize.x / 20.0f;
		float xs = grid.GetComponent<Grid>().cellSize.x / 2.0f + buffer;
		float ys = grid.GetComponent<Grid>().cellSize.y / 2.0f + buffer;

		List<Vector3> extremeties = new List<Vector3> {
			realSrc,
			realSrc + new Vector3(xs, ys, 0),
			realSrc + new Vector3(xs, -ys, 0),
			realSrc + new Vector3(-xs, -ys, 0),
			realSrc + new Vector3(-xs, ys, 0)
		};

		int res = src.ManhattanDistance(dest)*2;
		return extremeties.Select( ex => RaycastCost(ex, realDest, grid, resolution: res) ).Min();
	}

	private int RaycastCost(Vector3 src, Vector3 dest, WorldGrid grid, int resolution = 100) {
		HashSet<Vector3Int> alongRay = new HashSet<Vector3Int>();

		foreach (var step in src.SteppedInterpEx(dest, resolution)) {
			alongRay.Add( grid.Real2GridPos(step) );
		}

		// now return the count of tiles and the extra incurred cost
		return alongRay.Aggregate(0, (acc, it) =>  acc + grid.TerrainAt(it).occlusion ) + alongRay.Count;
	}

	public virtual void Display(WorldGrid grid) {
		foreach(Vector3Int nonField in grid.GetAllTilePos()) {
			if (field.ContainsKey(nonField)) continue;
			grid.HideAt(nonField);
		}
	}

	public virtual void ClearDisplay(WorldGrid grid) {
		grid.ResetAllHighlightTiles();
	}
}