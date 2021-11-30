using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BattleMap : MonoBehaviour
{
    [SerializeField] private PlayerInputController inputController;
    [SerializeField] private Tile mouseOverOverlayTile;
    private GridPosition recentMouseOver;

    private Tilemap overlayTilemap;
    private Tilemap baseTilemap;
    private Tilemap backgroundTilemap;
    
    public IEnumerable<GridPosition> Positions { get => GetPositions(baseTilemap); }
    private Dictionary<GridPosition, GridEntity> entityMap;

    void Awake() {
        Tilemap[] tilemaps = GetComponentsInChildren<Tilemap>();

        overlayTilemap = tilemaps[0];
        overlayTilemap.CompressBounds();
		overlayTilemap.RefreshAllTiles();

        baseTilemap = tilemaps[1];
        baseTilemap.CompressBounds();
		baseTilemap.RefreshAllTiles();

        backgroundTilemap = tilemaps[2];
        backgroundTilemap.CompressBounds();
		backgroundTilemap.RefreshAllTiles();

        // init
        // entityMap = new Dictionary<GridPosition, GridEntity>(new GridPosition.EqualityComparer());
        entityMap = new Dictionary<GridPosition, GridEntity>();
    }

    void Start() {
        inputController.MouseClickEvent += CheckMouseClick;
        inputController.MousePositionEvent += CheckMouseOver;

        // populate entityMap by checking the predetermined placements
        foreach (var gp in Positions) {
            entityMap[gp] = null;
        }
        foreach (GridEntity en in FindObjectsOfType<GridEntity>().OfType<IGridPosition>()) {
            if (entityMap[en.gridPosition] != null) {
                Debug.Log($"ERROR: CANNOT OVERRIDE PREVIOUS OCCUPANT {entityMap[en.gridPosition]} @ {en.gridPosition}");
            }
            entityMap[en.gridPosition] = en;
        }
    }

    public bool IsInBounds(GridPosition gp) {
        return entityMap.ContainsKey(gp);
    }

    public GridPosition WorldToGrid(Vector3 worldPosition) {
        Vector3Int gridVector = GetComponent<Grid>().WorldToCell(worldPosition);
        return new GridPosition(gridVector);
    }

    public Vector3 GridToWorld(GridPosition gridPosition) {
        return GetComponent<Grid>().CellToWorld(gridPosition);
    }

    private void CheckMouseClick(Vector3 screenPosition) {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        GridPosition gridPosition = WorldToGrid(worldPosition);
        Debug.Log($"BattleMap has seen that you clicked {screenPosition}, aka {worldPosition}, aka {gridPosition}");
    }

    private void CheckMouseOver(Vector3 screenPosition) {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        GridPosition gridPosition = WorldToGrid(worldPosition);

        if (recentMouseOver != gridPosition) {
            if (recentMouseOver != null) overlayTilemap.SetTile(recentMouseOver, null);
            if (IsInBounds(gridPosition)) {
                overlayTilemap.SetTile(gridPosition, mouseOverOverlayTile);
                recentMouseOver = gridPosition;
            }
        }
    }

    private static IEnumerable<GridPosition> GetPositions(Tilemap tilemap) {
		foreach (var pos in tilemap.cellBounds.allPositionsWithin) {
			Vector3Int v = new Vector3Int(pos.x, pos.y, pos.z);
			if (tilemap.HasTile(v)) yield return new GridPosition(v);
		}
	}
}
