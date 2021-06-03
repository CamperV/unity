using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Extensions;
using System.Linq;

public class FieldOfView
{
	public static int maxVisibility = 10;

	// this differs from a FlowField because it relies on line-of-site
	// as such, instead of pathfinding, we'll calculate the LoS for every involved tile
	public Vector3Int origin;
	public Dictionary<Vector3Int, int> field;

	public FieldOfView(Vector3Int _origin, int range, Overworld grid) {
		origin = _origin;
		field = RaycastField(origin, range, grid);
	}

	private Dictionary<Vector3Int, int> RaycastField(Vector3Int origin, int range, Overworld grid) {
		Dictionary<Vector3Int, int> _field = new Dictionary<Vector3Int, int> {
			[origin] = 0,
			/*[origin + Vector3Int.up] = 0,
			[origin + Vector3Int.left] = 0,
			[origin + Vector3Int.down ] = 0,
			[origin + Vector3Int.right] = 0,
			[origin + Vector3Int.up + Vector3Int.right] = 0,
			[origin + Vector3Int.up + Vector3Int.left] = 0,
			[origin + Vector3Int.down + Vector3Int.right] = 0,
			[origin + Vector3Int.down + Vector3Int.left] = 0*/
		};

		// reverse raycast
		foreach(Vector3Int v in origin.RadiateSquare(range)) {
			if (!grid.IsInBounds(v) || _field.ContainsKey(v)) continue;
			
			// draw a line between this tile and the origin
			// get all tiles along the line, and count up their occlusion + total with the number of tiles covered
			// interpolate for atleast twice the range, b/c sampling theroem?
			int occlusion = BresenhamCost(v, origin, grid);
			if (occlusion <= range) {
				_field[v] = occlusion;
			}
		}
		return _field;
	}
	
	public void Display(Overworld grid) {
		foreach(Vector3Int nonField in grid.GetAllTilePos()) {
			if (field.ContainsKey(nonField)) continue;
			grid.HideAt(nonField, intensity: 1.0f);
		}
	}

	public void ClearDisplay(Overworld grid) {
		grid.ResetAllHighlightTiles();
	}

	private int BresenhamCost(Vector3Int src, Vector3Int dest, Overworld grid) {
		Vector3Int unitDir = (dest - src).Unit();

		int acc = 0;
		foreach(var p in FieldOfView.BresenhamLine(src, dest)) {
			if(p == src) continue;

			// check diagonal squeezers
			/*
			int xOcc = grid.TerrainAt( p - unitDir.X() ).occlusion;
			int yOcc = grid.TerrainAt( p - unitDir.Y() ).occlusion;
			if (xOcc > 0 && yOcc > 0) {
				acc += Mathf.Min(xOcc, yOcc);
			}*/
			
			acc += grid.TerrainAt(p).occlusion + 1;
		}
		return acc;
	}

	public static IEnumerable<Vector3Int> BresenhamLine(Vector3Int src, Vector3Int dest) {
		void Swap(ref int a, ref int b) {
            int swap = a; a = b; b = swap;
		}
		int fromX = src.x, fromY = src.y;
		int toX = dest.x, toY = dest.y;
        bool vertical = Mathf.Abs(toY - fromY) > Mathf.Abs(toX - fromX);

		// reorient the problem for a shallow slope, in the x-increasing direction
        if (vertical) {
			Swap(ref fromX, ref fromY);
			Swap(ref toX, ref toY);
        }
        if (fromX > toX) {
			Swap(ref fromX, ref toX);
			Swap(ref fromY, ref toY);
        }

        int dx = toX - fromX;
        int dy = Mathf.Abs(toY - fromY);

        int error = dx / 2;
        int ys = (fromY < toY) ? 1 : -1;
        int y = fromY;

        for (int x = fromX; x <= toX; x++) {
			yield return (vertical) ? new Vector3Int(y, x, 0) : new Vector3Int(x, y, 0);

            error -= dy;
            if (error < 0) {
                y += ys;
                error += dx;
            }
        }
	}

	private int MinimumRaycastCost(Vector3Int src, Vector3Int dest, Overworld grid) {
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

	// this is way too expensive
	private int RaycastCost(Vector3 src, Vector3 dest, Overworld grid, int resolution = 10) {
		HashSet<Vector3Int> alongRay = new HashSet<Vector3Int>();

		foreach (var step in src.SteppedInterpEx(dest, resolution)) {
			alongRay.Add( grid.Real2GridPos(step) );
		}

		// now return the count of tiles and the extra incurred cost
		return alongRay.Aggregate(0, (acc, it) =>  acc + grid.TerrainAt(it).occlusion ) + alongRay.Count;
	}

	public static IEnumerable<Vector3Int> RaycastLine(Vector3 src, Vector3 dest, Overworld grid, int resolution = 10) {
		HashSet<Vector3Int> alongRay = new HashSet<Vector3Int>();

		foreach (var step in src.SteppedInterpEx(dest, resolution)) {
			alongRay.Add( grid.Real2GridPos(step) );
		}

		foreach (var v in alongRay) {
			yield return v;
		}
	}
}