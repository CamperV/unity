using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;

public class TacticsGrid : GameGrid
{
	// Y-Cell scale:
	// 0.5 is dimetric
	// 0.57735 this is true isometric

	private OverlayTile waypointOverlayTile;
	private OverlayTile selectionOverlayTile;

	public Dictionary<Vector2Int, Vector3Int> surface;
	public IEnumerable<Vector3Int> Surface { get => surface.Values; }
	
    protected override void Awake() {
		base.Awake();
		surface = new Dictionary<Vector2Int, Vector3Int>();

		waypointOverlayTile = PathOverlayIsoTile.GetTileWithSprite(1);
		selectionOverlayTile = (ScriptableObject.CreateInstance<SelectOverlayIsoTile>() as SelectOverlayIsoTile);
	}

	// for debug
	void Update() {
		if (Input.GetMouseButtonDown(0)) {
			GetMouseToGridPos();
		}
	}

	// IPathable definitions
	public override IEnumerable<Vector3Int> GetNeighbors(Vector3Int tilePos) {
		List<Vector2Int> cardinal = new List<Vector2Int> {
			Vector2Int.up, 		// N
			Vector2Int.right, 	// E
			Vector2Int.down, 	// S
			Vector2Int.left  	// W
		};
		
		foreach (Vector2Int cPos in cardinal) {
			Vector3Int pos = To3D(new Vector2Int(tilePos.x, tilePos.y) + cPos);
			if (IsInBounds(pos)) yield return pos;
		}
	}
	public override int EdgeCost(Vector3Int src, Vector3Int dest) {
        return (baseTilemap.GetTile(dest) as TacticsTile).cost;
    }
	
	public override bool IsInBounds(Vector3Int tilePos) {
		Vector2Int in2D = new Vector2Int(tilePos.x, tilePos.y);
		return surface.ContainsKey(in2D) && surface[in2D] == tilePos;
	}

	public override Vector3Int To3D(Vector2Int v) {
		if (surface.ContainsKey(v)) {
			return surface[v];
		} else {
			return new Vector3Int(v.x, v.y, 0);
		}
	}

	public override Vector3 Grid2RealPos(Vector3Int tilePos) {
		// offset because we need a unit to "sit on top of" a gridcell. This might be wrong, but we'll see?
		return GetComponent<Grid>().CellToWorld(tilePos + new Vector3Int(0, 0, 1));
	}
	
	public override Vector3Int Real2GridPos(Vector3 realPos) {
		return GetComponent<Grid>().WorldToCell(realPos);
	}

	public override Vector3Int GetMouseToGridPos() {
		Vector3 screenPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		float maxZ = baseTilemap.cellBounds.max.z;
		float zSize = GetComponent<Grid>().cellSize.z;

        for (float _z = maxZ; _z >= 0f; _z -= zSize) {
			var gridPos = Real2GridPos(new Vector3(screenPos.x, screenPos.y, _z));
			if (IsInBounds(gridPos)) return gridPos;
		}

		return Constants.unselectableVector3Int;
	}
	//
	// END OVERRIDE ZONE
	//

	// use this to cast from the camera to the Tilemap, and find the first tile that exists in our surface
	// This only "kind of" works, and is therefore deprecanted
	private Vector3Int? _Deprecated_CustomRaycastZ(Ray ray) {
		
		// we negate this step, because coming from the camera -> 0 it is looking for the wrong z value re: isometric tiles
		Vector3 origin = ray.origin;
		Vector3 final = ray.GetPoint(-ray.origin.z / ray.direction.z);
		Vector3 invOrigin = new Vector3(ray.origin.x, ray.origin.y, -ray.origin.z);

		// calculate the number of interpSteps based on distance + how many cells could exist here, then pad it by mult x 3
		float distance = (final.z - origin.z);	// distance from Camera to z=0
		Vector3 cellSize = GetComponent<Grid>().cellSize;
		int interpSteps = (int)(3f * (distance / cellSize.z));

		for (float i = 0f; i <= interpSteps; i++) {
			float interp = i / interpSteps;
			Vector3 step = Vector3.Lerp(invOrigin, final, interp);

			Vector3Int currentGridPos = Real2GridPos(step);
			if (IsInBounds(currentGridPos)) {
				return currentGridPos;
			}
		}
		return null;
	}

	public bool DimmableAt(Vector3Int tilePos) {
		TacticsTile tt = baseTilemap.GetTile(tilePos) as TacticsTile;
		return tt.dimmable;
	}

	public float ZHeightAt(Vector3Int tilePos) {
		TacticsTile tt = baseTilemap.GetTile(tilePos) as TacticsTile;
		return tt.zHeight;
	}

	public void SelectAtAlternate(Vector3Int tilePos) {
		OverlayAt(tilePos, waypointOverlayTile);
	}

	public virtual void ResetSelectionAtAlternate(Vector3Int tilePos, float fadeRate = 0.025f) {
		ResetOverlayAt(tilePos);
	}

	public override void UnderlayAt(Vector3Int tilePos, Color color) {
		underlayTilemap.SetTile(tilePos, selectionOverlayTile);
		TintTile(underlayTilemap, tilePos, color);
	}

	public override void ResetUnderlayAt(Vector3Int tilePos) {
		underlayTilemap.SetTile(tilePos, null);
		ResetTintTile(underlayTilemap, tilePos);
	}

	public void SetAppropriateTile(Vector3Int tilePos, TacticsTile tile) {
		surface[new Vector2Int(tilePos.x, tilePos.y)] = tilePos;
		baseTilemap.SetTile(tilePos, tile);
	}
	
	public Vector3Int GetDimensions() {
		return new Vector3Int(baseTilemap.cellBounds.xMax - baseTilemap.cellBounds.xMin,
							  baseTilemap.cellBounds.yMax - baseTilemap.cellBounds.yMin,
							  baseTilemap.cellBounds.zMax - baseTilemap.cellBounds.zMin);
	}

	public Vector3 GetCellRadius2D() {
		Vector3Int maxCell = new Vector3Int(baseTilemap.cellBounds.xMax, baseTilemap.cellBounds.yMax, 0);
		float mag = (baseTilemap.GetCellCenterWorld(maxCell) - GetGridCenterReal()).magnitude;
		return new Vector3(0, mag, 0);
	}
	
	public Vector3 GetGridCenterReal() {
		// interpolate the real world coordinates of the "middle" tiles
		Vector3Int floorCell = new Vector3Int((int)Mathf.Floor(baseTilemap.cellBounds.center.x-1),
											  (int)Mathf.Floor(baseTilemap.cellBounds.center.y-1), 0);
		Vector3Int ceilCell  = new Vector3Int((int)Mathf.Ceil(baseTilemap.cellBounds.center.x),
										   	  (int)Mathf.Ceil(baseTilemap.cellBounds.center.y), 0);
		return (baseTilemap.GetCellCenterWorld(floorCell) + baseTilemap.GetCellCenterWorld(ceilCell)) / 2.0f;
	}
	
	public Vector3 GetTilemapOrigin() {
		return baseTilemap.GetCellCenterWorld(baseTilemap.origin);
	}

	public Bounds GetBounds() {
		return baseTilemap.localBounds;
	}
}
