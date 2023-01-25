using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Extensions;

public class ItemMap : MonoBehaviour
{
    [SerializeField] private BattleMap battleMap;
    private Dictionary<GridPosition, MapItem> map;

    void Awake() {
        map = new Dictionary<GridPosition, MapItem>();
    }

    void Start() {
        // init map to have no blanks
        foreach (GridPosition gp in battleMap.Positions) {
            map[gp] = null;
        }

        // find all active objects that have a gridPosition
        // this happens only for activeUnits already in the hierarchy
        foreach (MapItem mapItem in GetComponentsInChildren<MapItem>()) {
            GridPosition startingGP = battleMap.ClosestGridPosition(mapItem.transform.position);
            PlaceItem(mapItem, startingGP);
        }
    }

    public MapItem MapItemAt(GridPosition gp) {
        return map[gp];
    }

    public void CheckForMapItem(Unit unit) {
        MapItemAt(unit.gridPosition)?.OnUnitEnter(unit);
    }

    // accessible area
    // move a mapItem into a gridPosition, transform and all. ONly if not reserved
    public void PlaceItem(MapItem mapItem, GridPosition gp) {
        if (map[gp] == null) {
            GridPosition prevGridPosition = mapItem.gridPosition;

            AlignItem(mapItem, gp);
            map[mapItem.gridPosition] = mapItem;

            // if we're moving it
            if (prevGridPosition != null) map[prevGridPosition] = null;

        } else {
            if (map[gp] == mapItem) AlignItem(mapItem, gp);
        }
    }
        
    private void AlignItem(MapItem mapItem, GridPosition gp) {
        mapItem.gridPosition = gp;
        mapItem.transform.position = battleMap.GridToWorld(gp);
    }
}
