using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using Extensions;

public class BattleMap : MonoBehaviour, IPathable<GridPosition>, IGrid<GridPosition>
{
    // publicly available events
    public delegate void GridInteraction(GridPosition gridPosition);
    public event GridInteraction InteractEvent;
    public event GridInteraction AuxiliaryInteractEvent_0;
    public event GridInteraction AuxiliaryInteractEvent_1;
    public event GridInteraction AuxiliaryInteractEvent_2;
    public event GridInteraction GridMouseOverEvent;

    // for pathing purposing
    [SerializeField] private TerrainSystem terrainSystem;

    [SerializeField] private VisualTile mouseOverOverlayTile;
    [SerializeField] private PathTileSet pathTileSet;
    
    private GridPosition recentMouseOver;
    public GridPosition CurrentMouseGridPosition { get => recentMouseOver; }
    public bool MouseInBounds { get => IsInBounds(recentMouseOver); }

    [SerializeField] private Tilemap overlayTilemap;
    [SerializeField] private Tilemap highlightTilemap;
    [SerializeField] private Tilemap gridlinesTilemap;
    [SerializeField] private Tilemap baseTilemap;
    [SerializeField] private Tilemap visualTilemap;

    [SerializeField] private VisualTile baseHighlightTile;
    
    private HashSet<GridPosition> _Positions;
    public HashSet<GridPosition> Positions {
        get {
            if (_Positions == null) _Positions = new HashSet<GridPosition>(GetTilemapPositions(baseTilemap));
            return _Positions;
        }
    }

    // this is used/updated to disable clicking on the battleMap when interacting with UI elements
    private bool disableInteraction;

    void Awake() {
        overlayTilemap.CompressBounds();
		overlayTilemap.RefreshAllTiles();

        gridlinesTilemap.CompressBounds();
		gridlinesTilemap.RefreshAllTiles();

        highlightTilemap.CompressBounds();
		highlightTilemap.RefreshAllTiles();

        baseTilemap.CompressBounds();
		baseTilemap.RefreshAllTiles();
    }

    void Start() {
        ResetHighlight();
    }

    void Update() {
        disableInteraction = EventSystem.current.IsPointerOverGameObject();
    }

    public bool IsInBounds(GridPosition gp) {
        return Positions.Contains(gp);
    }

    // IGrid definitions
    public GridPosition WorldToGrid(Vector3 worldPosition) {
        return GetComponent<Grid>().WorldToCell(worldPosition);
    }

    // we use Tilemap here because otherwise, Grid aligns to vertices
    public Vector3 GridToWorld(GridPosition gridPosition) {
        return baseTilemap.GetCellCenterWorld(gridPosition);
    }

    // this is particular in that it only returns InBounds GridPositions
    public GridPosition ClosestGridPosition(Vector3 worldPosition) {
        GridPosition gp = WorldToGrid(worldPosition);
        if (IsInBounds(gp)) return gp;

        int minDist = Positions.Min(it => it.ManhattanDistance(gp));
        return Positions.First(it => it.ManhattanDistance(gp) == minDist);
    }

    public void CheckLeftMouseClick(Vector3 screenPosition) {
        if (disableInteraction) return;
        //

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        GridPosition gridPosition = WorldToGrid(worldPosition);

        if (IsInBounds(gridPosition)) {
            InteractEvent?.Invoke(gridPosition);
        }
    }
    
    public void CheckRightMouseClick(Vector3 screenPosition) {
        if (disableInteraction) return;
    }

    public void CheckAuxiliaryInteract(Vector3 screenPosition) {
        if (disableInteraction) return;
        //

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        GridPosition gridPosition = WorldToGrid(worldPosition);

        if (IsInBounds(gridPosition)) {
            AuxiliaryInteractEvent_2?.Invoke(gridPosition);
        }
    }

    public void CheckMouseOver(Vector3 screenPosition) {
        if (disableInteraction) return;
        //

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        GridPosition gridPosition = WorldToGrid(worldPosition);

        if (recentMouseOver != gridPosition) {
            if (recentMouseOver != null) overlayTilemap.SetTile(recentMouseOver, null);
            if (IsInBounds(gridPosition)) {
                overlayTilemap.SetTile(gridPosition, mouseOverOverlayTile);
                
                //
                GridMouseOverEvent?.Invoke(gridPosition);
            }
            recentMouseOver = gridPosition;
        }
    }

    public void CheckLeftMouseHoldStart(Vector3 screenPosition) {
        if (disableInteraction) return;
        //

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        GridPosition gridPosition = WorldToGrid(worldPosition);

        if (IsInBounds(gridPosition)) {
            
            AuxiliaryInteractEvent_0?.Invoke(gridPosition);
        }
    }

    public void CheckLeftMouseHoldEnd(Vector3 screenPosition) {
        if (disableInteraction) return;
        //

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        GridPosition gridPosition = WorldToGrid(worldPosition);

        if (IsInBounds(gridPosition)) {
            AuxiliaryInteractEvent_1?.Invoke(gridPosition);
        }
    }

    private static IEnumerable<GridPosition> GetTilemapPositions(Tilemap tilemap) {
		foreach (var pos in tilemap.cellBounds.allPositionsWithin) {
			Vector3Int v = new Vector3Int(pos.x, pos.y, pos.z);
			if (tilemap.HasTile(v)) yield return new GridPosition(v);
		}
	}

    // IPathable definitions
    public IEnumerable<GridPosition> GetNeighbors(GridPosition origin) {
        GridPosition up    = origin + (GridPosition)Vector2Int.up;
        GridPosition right = origin + (GridPosition)Vector2Int.right;
        GridPosition down  = origin + (GridPosition)Vector2Int.down;
        GridPosition left  = origin + (GridPosition)Vector2Int.left;
        if (IsInBounds(up))    yield return up;
        if (IsInBounds(right)) yield return right;
        if (IsInBounds(down))  yield return down;
        if (IsInBounds(left))  yield return left;
    }

    // IPathable definitions
	public int BaseCost(GridPosition gp) {
        return terrainSystem.TerrainAt(gp).cost;
	}

	public void Highlight(GridPosition gp, Color color) {
		if (highlightTilemap.HasTile((Vector3Int)gp)) {
			highlightTilemap.SetTileFlags(gp, TileFlags.None);
			highlightTilemap.SetColor(gp, color);
		}
	}
	
    public void ResetHighlight() {
        highlightTilemap.color = Color.white;

        foreach (var pos in highlightTilemap.cellBounds.allPositionsWithin) {
            Vector3Int v = new Vector3Int(pos.x, pos.y, pos.z);
            if (highlightTilemap.HasTile(v)) {
                highlightTilemap.SetTileFlags(v, TileFlags.None);
                highlightTilemap.SetColor(v, Color.white.WithAlpha(0f));
            }
        }
	}

    public void SetHighlightTile(GridPosition gp, VisualTile vt) {
        if (highlightTilemap.HasTile((Vector3Int)gp)) {
            highlightTilemap.SetTile((Vector3Int)gp, vt);
        }
    }

    public void ResetHighlightTiles() {
        foreach (var pos in highlightTilemap.cellBounds.allPositionsWithin) {
            Vector3Int v = new Vector3Int(pos.x, pos.y, pos.z);
            if (highlightTilemap.HasTile(v)) {
                highlightTilemap.SetTile(v, baseHighlightTile);
            }
        }
    }
    
	public void DisplayPath(Path<GridPosition> path) {
		pathTileSet.DisplayPath(overlayTilemap, path);
	}

	public void DisplayPath(Path<GridPosition> path, List<GridPosition> waypoints) {
		pathTileSet.DisplayPath(overlayTilemap, path, waypoints);
	}

    // this uses Vector3Int, to reach that special forbidden zone where GridPosition's cannot reach normally
	public void ClearDisplayPath() {
        pathTileSet.ClearDisplayPath(overlayTilemap);
	}
}
