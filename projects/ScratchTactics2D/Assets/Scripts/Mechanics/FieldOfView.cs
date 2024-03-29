﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Extensions;
using System.Linq;

public class FieldOfView
{
	public static int maxVisibility = 10;	// gets interpolated for occlusion/display values
	private static float intensity = 0.20f; // for hiding/revealing tiles
	private static Overworld overworld { get => GameManager.inst.overworld; }

	private static Dictionary<Enum.VisibleState, int> visThresholds = new Dictionary<Enum.VisibleState, int>{
		[Enum.VisibleState.visible] 		  = 1,
		[Enum.VisibleState.partiallyObscured] = 3,
		[Enum.VisibleState.obscured] 		  = 7,
		[Enum.VisibleState.hidden]			  = 10
	};

	// this differs from a FlowField because it relies on line-of-site
	// as such, instead of pathfinding, we'll calculate the LoS for every involved tile
	public Vector3Int origin;
	public Dictionary<Vector3Int, Enum.VisibleState> field;

	public FieldOfView(Vector3Int _origin, int range) {
		origin = _origin;
		//
		field = RaycastField(origin, range);
	}

	public Enum.VisibleState OcclusionAt(Vector3Int pos) {
		if (field.ContainsKey(pos)) {
			return field[pos];
		} else {
			return Enum.VisibleState.hidden;
		}
	}

	private Dictionary<Vector3Int, Enum.VisibleState> RaycastField(Vector3Int origin, int range) {
		Dictionary<Vector3Int, Enum.VisibleState> _field = new Dictionary<Vector3Int, Enum.VisibleState> {
			[origin] = 0
		};

		// reverse raycast
		foreach(Vector3Int v in origin.RadiateCircle(range)) {
			if (!overworld.IsInBounds(v) || _field.ContainsKey(v)) continue;
			
			// draw a line between this tile and the origin
			// get all tiles along the line, and count up their occlusion + total with the number of tiles covered
			int occlusion = BresenhamCost(v, origin);

			// snap to certain levels of occlusion: Visible, Obscured, Hidden
			// switch (occlusion) {
			// 	case 0:				// Visible
			// 	case 1:
			// 		occlusion = Enum.VisibleState.visible;
			// 		break;
			// 	case 2:
			// 	case 3:
			// 		occlusion = Enum.VisibleState.partiallyObscured;	// Partially Obscured
			// 		break;
			// 	case 4:
			// 	case 5:
			// 	case 6:
			// 	case 7:
			// 		occlusion = Enum.VisibleState.obscured; // Obscured
			// 		break;
			// 	case 8:
			// 	case 9:
			// 	case 10:
			// 		occlusion = Enum.VisibleState.hidden; // Hidden
			// 		break;
			// }

			Enum.VisibleState visibility = Enum.VisibleState.hidden;
			if (occlusion <= visThresholds[Enum.VisibleState.visible]) {
				visibility = Enum.VisibleState.visible;
			} else if (occlusion <= visThresholds[Enum.VisibleState.partiallyObscured]) {
				visibility = Enum.VisibleState.partiallyObscured;
			} else if (occlusion <= visThresholds[Enum.VisibleState.obscured]) {
				visibility = Enum.VisibleState.obscured;
			} else if (occlusion <= visThresholds[Enum.VisibleState.hidden]) {
				visibility = Enum.VisibleState.hidden;
			}
			_field[v] = visibility;
		}
		return _field;
	}

	private int BresenhamCost(Vector3Int v, Vector3Int origin) {
		int acc = 0;
		int vAlt = overworld.TerrainAt(v).altitude;
		int originAlt = overworld.TerrainAt(origin).altitude;
		int vDiff = Mathf.Abs(originAlt - vAlt);

		foreach (Vector3Int p in BresenhamLine(v, origin)) {
			if(p == origin || p == v) continue;
			int pAlt = overworld.TerrainAt(p).altitude;

			// don't be occluded by something that you're taller than
			if (vAlt > pAlt) continue;
			
			// if you're occluded by something taller
			if (pAlt > vAlt) return FieldOfView.maxVisibility;
			
			// else, merely you're occluded or in range
			acc += overworld.TerrainAt(p).occlusion;
		}
		return Mathf.Min(FieldOfView.maxVisibility, acc);
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
		// keep track of the occlusion assigned to each revealed tile
		// always choose the smallest, if it already exists
		// this is to make sure that a partially reveal 2x2 tileRef will always be revealed
		Dictionary<Vector3Int, int> assignedOcc = new Dictionary<Vector3Int, int>();

		foreach (var pos in overworld.Positions) {
			Vector3Int posFromTerrain = overworld.TerrainAt(pos).tileRefPosition;

			// if you need to be revealed:
			if (field.ContainsKey(pos)) {
				int occlusion = visThresholds[field[pos]];

				// always take the smallest occlusion if previously stored
				if (assignedOcc.ContainsKey(posFromTerrain)) {
					occlusion = Mathf.Min(occlusion, assignedOcc[posFromTerrain]);
				}
				assignedOcc[posFromTerrain] = occlusion;

				// interpolate the occlusion via maxVisibility and use the float for intensity
				float i = Mathf.InverseLerp(0, maxVisibility, maxVisibility - occlusion);
				HideAt(posFromTerrain, Mathf.Lerp(intensity, 1.0f, i));
			} else if (GlobalPlayerState.previouslyRevealedOverworldPositions.Contains(pos)) {
				HideAt(posFromTerrain, intensity);
			} else {
				HideAt(posFromTerrain, 0f);
			}
		}
	}

	private void HideAt(Vector3Int tilePos, float _intensity) {
		overworld.HighlightTile(tilePos, (_intensity*Color.white).WithAlpha(1.0f));
	}

	// deprecated
	private void CastAltitudeShadows() {		
		field = new Dictionary<Vector3Int, Enum.VisibleState>();

		// cast over several rounds, corresponding to altitude
		int minAltitude = overworld.Terrain.Min(it => it.altitude);
		int maxAltitude = overworld.Terrain.Max(it => it.altitude);

		//for (int alt = maxAltitude; alt > 0; alt--) {
		for (int alt = minAltitude+1; alt < maxAltitude+1; alt++) {	
			HashSet<Vector2Int> validSet = new HashSet<Vector2Int>(
				overworld.Terrain.Where(it => it.altitude == alt || it.altitude == alt - 1)
								 .Select(it => new Vector2Int(it.position.x, it.position.y))
								 .ToList()
			);

			new AltitudeShadowCaster(origin, maxVisibility,
				/* IsOpaque */
				(x, y) => {
					return overworld.TerrainAt(new Vector3Int(x, y, 0)).altitude == alt;
				},

				/* SetFOV */
				(x, y) => {
					if (validSet.Contains(new Vector2Int(x, y))) {
						field[new Vector3Int(x, y, 0)] = Enum.VisibleState.visible;
					}
				}
			).CastShadows();
		}
	}
}