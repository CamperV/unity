using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Extensions;

public class UnitMap : MonoBehaviour
{
    //publicly available events
    public delegate void BoardState();
    public event BoardState NewBoardStateEvent;
    public UnityEvent<Unit, GridPosition> EnterTileEvent;
    public UnityEvent<Unit, GridPosition> ExitTileEvent;

    private BattleMap battleMap;

    private Dictionary<GridPosition, Unit> map;
    private Dictionary<GridPosition, Unit> reservations;

    public IEnumerable<Unit> Units => map.Values.Where(u => u != null);

    void Awake() {
        battleMap = GetComponentInChildren<BattleMap>();
        map = new Dictionary<GridPosition, Unit>();
        reservations = new Dictionary<GridPosition, Unit>();
    }

    void Start() {
        // init map to have no blanks
        foreach (GridPosition gp in battleMap.Positions) {
            map[gp] = reservations[gp] = null;
        }

        // find all active objects that have a gridPosition
        // this happens only for activeUnits already in the hierarchy
        foreach (Unit unit in GetComponentsInChildren<Unit>()) {
            GridPosition startingGP = battleMap.ClosestGridPosition(unit.transform.position);
            MoveUnit(unit, startingGP, newBoardEvent: false);
        }
    }

    public void InsertUnitsAtSpawnMarkers(ICollection<PlayerUnit> units) {        
        List<GridPosition> spawnMarkers = new List<GridPosition>();
        foreach (SpawnMarker sm in GetComponentsInChildren<SpawnMarker>()) {
            spawnMarkers.Add( battleMap.ClosestGridPosition(sm.transform.position) );
        }

        Debug.Assert(units.Count <= spawnMarkers.Count);

        foreach (PlayerUnit unit in units) {
            if (spawnMarkers.Count > 0) {
                GridPosition startingPos = spawnMarkers.PopAt<GridPosition>(0);
                MoveUnit(unit, startingPos, newBoardEvent: false);

            // this shouldn't be reachable
            } else {
                Debug.Log($"You dont have enough spawnMarkers to spawn all Units properly");
            }
        }
    }

    public Unit UnitAt(GridPosition gp) {
        return map[gp];
    }

    public bool ReservedAt(GridPosition gp) {
        return reservations[gp] != null;
    }
    
    public bool CanMoveInto(GridPosition gp) {
        return map[gp] == null && reservations[gp] == null;
    }

    // accessible area
    // move a unit into a gridPosition, transform and all. ONly if not reserved
    public void MoveUnit(Unit unit, GridPosition gp, bool newBoardEvent = true) {
        if (map[gp] == null && (reservations[gp] == null || reservations[gp] == unit)) {
            GridPosition prevGridPosition = unit.gridPosition;
            AlignUnit(unit, gp);

            map[unit.gridPosition] = unit;

            if (prevGridPosition != null) map[prevGridPosition] = null;
            if (reservations[gp] == unit) reservations[gp] = null;

            // since this was successful, trigger the new state event
            if (newBoardEvent) NewBoardStateEvent.Invoke();

            // trigger relevant Tile Events/TerrainEffects
            ExitTileEvent?.Invoke(unit, prevGridPosition);
            EnterTileEvent?.Invoke(unit, unit.gridPosition);

        } else {
            if (map[gp] == unit || reservations[gp] == unit) AlignUnit(unit, gp);
            else Debug.LogError($"Failed to move {unit} into occupied GP {gp}");
        }
    }

    // ie, Move the unit in the map, but don't move it's transform yet
    public void ReservePosition(Unit unit, GridPosition gp) {
        if (map[gp] == null && reservations[gp] == null) {
            reservations[gp] = unit;
        } else {
            Debug.Log($"{unit} cannot reserve {gp}, {map[gp]} exists there");
        }
    }

    public void ClearReservation(GridPosition gp) {
        reservations[gp] = null;
    }

    public void ClearPosition(GridPosition gp) {
        map[gp] = null;
        reservations[gp] = null;
        NewBoardStateEvent.Invoke();  
    }
        
    public void AlignUnit(Unit unit, GridPosition gp) {
        unit.gridPosition = gp;
        unit.transform.position = battleMap.GridToWorld(gp);
    }

    public void ApplyTerrainEffects(GridPosition gridPosition) {
        Unit unit = UnitAt(gridPosition);

        // trigger relevant Tile Events/TerrainEffects
        if (unit != null) {
            ExitTileEvent?.Invoke(unit, unit.gridPosition);
            EnterTileEvent?.Invoke(unit, unit.gridPosition);
        }
    }
}
