using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Extensions;

public class BattleMap : MonoBehaviour, IPathable<GridPosition>, IGrid<GridPosition>
{
    // publicly available events
    public delegate void GridInteraction(GridPosition gridPosition);
    public event GridInteraction InteractEvent;
    public event GridInteraction AuxiliaryInteractEvent_0;
    public event GridInteraction AuxiliaryInteractEvent_1;
    public event GridInteraction AuxiliaryInteractEvent_2;

    public delegate void TerrainInteraction(TerrainTile tt);
    public event TerrainInteraction TerrainMouseOverEvent;
    //

    [SerializeField] private Tile mouseOverOverlayTile;
    [SerializeField] private Tile pathOverlayTile;
    [SerializeField] private Tile pathEndOverlayTile;
    
    private GridPosition recentMouseOver;
    public GridPosition CurrentMouseGridPosition { get => recentMouseOver; }
    public bool MouseInBounds { get => IsInBounds(recentMouseOver); }

    private Tilemap overlayTilemap;
    private Tilemap highlightTilemap;
    private Tilemap baseTilemap;
    private Tilemap backgroundTilemap;
    
    private HashSet<GridPosition> _Positions;
    public HashSet<GridPosition> Positions {
        get {
            if (_Positions == null) _Positions = new HashSet<GridPosition>(GetTilemapPositions(baseTilemap));
            return _Positions;
        }
    }

    void Awake() {
        Tilemap[] tilemaps = GetComponentsInChildren<Tilemap>();

        overlayTilemap = tilemaps[0];
        overlayTilemap.CompressBounds();
		overlayTilemap.RefreshAllTiles();

        highlightTilemap = tilemaps[1];
        highlightTilemap.CompressBounds();
		highlightTilemap.RefreshAllTiles();

        baseTilemap = tilemaps[2];
        baseTilemap.CompressBounds();
		baseTilemap.RefreshAllTiles();

        backgroundTilemap = tilemaps[3];
        backgroundTilemap.CompressBounds();
		backgroundTilemap.RefreshAllTiles();
    }

    void Start() {
        ResetHighlight();
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
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        GridPosition gridPosition = WorldToGrid(worldPosition);
        // Debug.Log($"BattleMap has seen that you clicked {screenPosition}, aka {worldPosition}, aka {gridPosition} [InBounds = {IsInBounds(gridPosition)}]");

        if (IsInBounds(gridPosition)) {
            InteractEvent?.Invoke(gridPosition);
        }
    }
    
    public void CheckRightMouseClick(Vector3 screenPosition) {
        // Debug.Log($"BattleMap has seen that you right-clicked {screenPosition}");
    }

    public void CheckMiddleMouseClick(Vector3 screenPosition) {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        GridPosition gridPosition = WorldToGrid(worldPosition);
        // Debug.Log($"BattleMap has seen that you middle-clicked {screenPosition}, aka {worldPosition}, aka {gridPosition} [InBounds = {IsInBounds(gridPosition)}]");

        if (IsInBounds(gridPosition)) {
            AuxiliaryInteractEvent_2?.Invoke(gridPosition);
        }
    }

    public void CheckMouseOver(Vector3 screenPosition) {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        GridPosition gridPosition = WorldToGrid(worldPosition);

        if (recentMouseOver != gridPosition) {
            if (recentMouseOver != null) overlayTilemap.SetTile(recentMouseOver, null);
            if (IsInBounds(gridPosition)) {
                overlayTilemap.SetTile(gridPosition, mouseOverOverlayTile);
                
                //
                TerrainMouseOverEvent?.Invoke(TerrainAt(gridPosition));
            }
            recentMouseOver = gridPosition;
        }
    }

    public void CheckLeftMouseHoldStart(Vector3 screenPosition) {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        GridPosition gridPosition = WorldToGrid(worldPosition);
        // Debug.Log($"BattleMap has seen that you started holding mouse @ {screenPosition}, aka {worldPosition}, aka {gridPosition} [InBounds = {IsInBounds(gridPosition)}]");

        if (IsInBounds(gridPosition)) {
            AuxiliaryInteractEvent_0?.Invoke(gridPosition);
        }
    }

    public void CheckLeftMouseHoldEnd(Vector3 screenPosition) {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        GridPosition gridPosition = WorldToGrid(worldPosition);
        // Debug.Log($"BattleMap has seen that you released mouse @ {screenPosition}, aka {worldPosition}, aka {gridPosition} [InBounds = {IsInBounds(gridPosition)}]");

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

	public int BaseCost(GridPosition gp) {
        return (baseTilemap.GetTile(gp) as TerrainTile).cost;
	}

    public TerrainTile TerrainAt(GridPosition gp) {
        return (baseTilemap.GetTile(gp) as TerrainTile);
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
    
	public void DisplayPath(Path<GridPosition> path) {
		foreach (GridPosition gp in path.Unwind()) {
            Vector3Int as_V = new Vector3Int(gp.x, gp.y, -1);
			overlayTilemap.SetTile(as_V, pathOverlayTile);
		}

        Vector3Int as_V2 = new Vector3Int(path.End.x, path.End.y, -1);
        overlayTilemap.SetTile(as_V2, pathEndOverlayTile);
	}

    // this uses Vector3Int, to reach that special forbidden zone where GridPosition's cannot reach normally
	public void ClearDisplayPath() {
        overlayTilemap.CompressBounds();

        foreach (Vector3Int v in overlayTilemap.cellBounds.allPositionsWithin) {
            if (v.z == -1) overlayTilemap.SetTile(v, null);
        }
	}
}
