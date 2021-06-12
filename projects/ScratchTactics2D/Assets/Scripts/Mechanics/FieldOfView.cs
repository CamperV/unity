using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Extensions;
using System.Linq;

public class FieldOfView
{
	public static int maxVisibility = 4; // in tiles, includes the origin in radius
	public static Overworld overworld { get => GameManager.inst.overworld; }
	private static float intensity = 0.15f; // for hiding/revealing tiles

	// this differs from a FlowField because it relies on line-of-site
	// as such, instead of pathfinding, we'll calculate the LoS for every involved tile
	public Vector3Int origin;
	public Dictionary<Vector3Int, int> field;

	public FieldOfView(Vector3Int _origin, int range) {
		origin = _origin;
		field = RadialField(origin, range);
	}

	private Dictionary<Vector3Int, int> RadialField(Vector3Int origin, int range) {
		Dictionary<Vector3Int, int> _field = new Dictionary<Vector3Int, int> {
			[origin] = 0
		};

		foreach(Vector3Int v in origin.RadiateSquare(range)) {
			if (!overworld.IsInBounds(v) || !Visible(v, range)) continue;
			if (VisibleCloseRange(v)) {
				_field[v] = 1;
				continue;
			}

			// hide if you're a forest which is surrounded by forests
			if (overworld.TypeAt(v).MatchesType(typeof(Forest))) {
				bool breakOut = true;
				foreach (var t in TerrainPatternShape.NoCenterPlus.YieldPattern(v)) {
					if (overworld.IsInBounds(t)) {
						breakOut &= overworld.TypeAt(t).MatchesType(typeof(Forest));
					}
				}
				if (breakOut) continue;
			}

			// by default, you are visible within the circle
			_field[v] = 1;
		}
		return _field;
	}

	private bool Visible(Vector3Int p, int range) {
		Vector3Int v = p - origin;
		return (v.x*v.x) + (v.y*v.y) <= (range*range);
	}

	private bool VisibleCloseRange(Vector3Int p) {
		Vector3Int v = p - origin;
		return (v.x*v.x) + (v.y*v.y) <= (2*2);
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
		int acc = 0;
		foreach (var p in BresenhamLine(src, dest)) {
			if(p == src) continue;
			
			acc += overworld.TerrainAt(p).occlusion + 1;
		}
		return acc;
	}

	private bool IsOccluded(Vector3Int src, Vector3Int dest) {
		// from origin to target tile
		int targetHeight = overworld.TerrainAt(src).altitude;
		foreach (var p in BresenhamLine(dest, src)) {
			if (p == src) continue;
			int h = overworld.TerrainAt(p).altitude;
			if (targetHeight == 0) {
				if (h > targetHeight) return true;
			} else {
				if (h >= targetHeight) return true;
			}
		}
		return false;
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

	public static IEnumerable<Pair<Vector3Int, float>> BresenhamLineAA(Vector3Int src, Vector3Int dest) {
		int x0 = src.x, x1 = dest.x;
		int y0 = src.y, y1 = dest.y;
		int dx = Mathf.Abs(x1 - x0), sx = (x0 < x1) ? 1 : -1;
		int dy = Mathf.Abs(y1 - y0), sy = (y0 < y1) ? 1 : -1;

		int e2, x2;
		int err = dx - dy;
		float ed = (dx + dy == 0f) ? 1f : Mathf.Sqrt((float)dx*dx + (float)dy*dy);

		while (true) {
			yield return new Pair<Vector3Int, float>(new Vector3Int(x0, y0, 0), Mathf.Abs(err-dx+dy)/ed);

			e2 = err;
			x2 = x0;

			// x
			if (e2*2 >= -dx) {
				if (x0 == x1) break;
				if (e2 + dy < ed) {
					yield return new Pair<Vector3Int, float>(new Vector3Int(x0, y0+sy, 0), (e2+dy)/ed);
				}
				err -= dy;
				x0 += sx;
			}

			// y
			if (e2*2 <= dy) {
				if (y0 == y1) break;
				if (dx - e2 < ed) {
					yield return new Pair<Vector3Int, float>(new Vector3Int(x2+sx, y0, 0), (dx-e2)/ed);
				}
				err += dx;
				y0 += sy;
			}
		}
	}

	public void Display() {
		return; 
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

	public void HideAt(Vector3Int tilePos) {
		Vector3Int posFromTerrain = overworld.TerrainAt(tilePos).tileRefPosition;
		overworld.HighlightTile(posFromTerrain, (intensity*Color.white).WithAlpha(1.0f));
	}
	
	public void RevealAt(Vector3Int tilePos) {
		Vector3Int posFromTerrain = overworld.TerrainAt(tilePos).tileRefPosition;
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