using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Extensions;
using System.Linq;

public class FieldOfView
{
	public static int maxVisibility = 10; // in tiles
	public static Overworld overworld { get => GameManager.inst.overworld; }
	private static float intensity = 0.15f; // for hiding/revealing tiles

	// this differs from a FlowField because it relies on line-of-site
	// as such, instead of pathfinding, we'll calculate the LoS for every involved tile
	public Vector3Int origin;
	public Dictionary<Vector3Int, int> field;

	public FieldOfView(Vector3Int _origin, int range) {
		origin = _origin;
		field = RaycastField(origin, range);
	}

	private Dictionary<Vector3Int, int> RaycastField(Vector3Int origin, int range) {
		Dictionary<Vector3Int, int> _field = new Dictionary<Vector3Int, int> {
			[origin] = 0
		};

		// reverse raycast
		foreach(Vector3Int v in origin.RadiateSquare(range)) {
			if (!overworld.IsInBounds(v) || _field.ContainsKey(v)) continue;
			
			// draw a line between this tile and the origin
			// get all tiles along the line, and count up their occlusion + total with the number of tiles covered
			int occlusion = BresenhamCost(v, origin);
			if (occlusion <= range) {
				_field[v] = occlusion;
			}
		}
		return _field;
	}

	private int BresenhamCost(Vector3Int src, Vector3Int dest) {
		Vector3Int unitDir = (dest - src).Unit();

		int acc = 0;
		foreach(var p in BresenhamLine(src, dest)) {
			if(p == src) continue;

			// check diagonal squeezers
			/*
			int xOcc = overworld.TerrainAt( p - unitDir.X() ).occlusion;
			int yOcc = overworld.TerrainAt( p - unitDir.Y() ).occlusion;
			if (xOcc > 0 && yOcc > 0) {
				acc += Mathf.Min(xOcc, yOcc);
			}*/
			
			acc += overworld.TerrainAt(p).occlusion + 1;
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

	public void Display() {
		// need to go over two passes, or at least in a certain order
		// this is because the _2x2TileRefs will reveal out of order
		foreach (var pos in overworld.Positions) {
			HideAt(pos);
		}
		foreach (var pos in overworld.Positions) {
			if (field.ContainsKey(pos)) {
				RevealAt(pos);
			}
		}
	}

	public static void HideAt(Vector3Int tilePos) {
		Vector3Int posFromTerrain = overworld.TerrainAt(tilePos).position;
		overworld.HighlightTile(posFromTerrain, (intensity*Color.white).WithAlpha(1.0f));
	}
	
	public static void RevealAt(Vector3Int tilePos) {
		Vector3Int posFromTerrain = overworld.TerrainAt(tilePos).position;
		overworld.ResetHighlightTile(posFromTerrain);
	}

	// DEPRECATED
	// private int MinimumRaycastCost(Vector3Int src, Vector3Int dest) {
	// 	Vector3 realSrc = overworld.Grid2RealPos(src);
	// 	Vector3 realDest = overworld.Grid2RealPos(dest);
	// 	float buffer = overworld.GetComponent<Grid>().cellSize.x / 20.0f;
	// 	float xs = overworld.GetComponent<Grid>().cellSize.x / 2.0f + buffer;
	// 	float ys = overworld.GetComponent<Grid>().cellSize.y / 2.0f + buffer;

	// 	List<Vector3> extremeties = new List<Vector3> {
	// 		realSrc,
	// 		realSrc + new Vector3(xs, ys, 0),
	// 		realSrc + new Vector3(xs, -ys, 0),
	// 		realSrc + new Vector3(-xs, -ys, 0),
	// 		realSrc + new Vector3(-xs, ys, 0)
	// 	};

	// 	int res = src.ManhattanDistance(dest)*2;
	// 	return extremeties.Select( ex => RaycastCost(ex, realDest, resolution: res) ).Min();
	// }

	// // this is way too expensive
	// private int RaycastCost(Vector3 src, Vector3 dest, int resolution = 10) {
	// 	HashSet<Vector3Int> alongRay = new HashSet<Vector3Int>();

	// 	foreach (var step in src.SteppedInterpEx(dest, resolution)) {
	// 		alongRay.Add( overworld.Real2GridPos(step) );
	// 	}

	// 	// now return the count of tiles and the extra incurred cost
	// 	return alongRay.Aggregate(0, (acc, it) =>  acc + overworld.TerrainAt(it).occlusion ) + alongRay.Count;
	// }

	// public static IEnumerable<Vector3Int> RaycastLine(Vector3 src, Vector3 dest, int resolution = 10) {
	// 	HashSet<Vector3Int> alongRay = new HashSet<Vector3Int>();

	// 	foreach (var step in src.SteppedInterpEx(dest, resolution)) {
	// 		alongRay.Add( overworld.Real2GridPos(step) );
	// 	}

	// 	foreach (var v in alongRay) {
	// 		yield return v;
	// 	}
	// }
}