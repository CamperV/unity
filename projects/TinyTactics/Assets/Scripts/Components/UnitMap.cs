using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UnitMap : MonoBehaviour
{
    private BattleMap battleMap;

    private Dictionary<GridPosition, Unit> map;
    private Dictionary<GridPosition, Unit> reservations;

    public List<GridPosition> currentMap = new List<GridPosition>();
    public List<GridPosition> currentRes = new List<GridPosition>();

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
        // this happens only for Entities already in the hierarchy
        foreach (Unit unit in GetComponentsInChildren<Unit>()) {
            GridPosition startingGP = battleMap.ClosestGridPosition(unit.transform.position);
            MoveUnit(unit, startingGP);
        }
    }

    void Update() {
        currentMap = map.Keys.Where(k => map[k] != null).ToList();
        currentRes = reservations.Keys.Where(k => reservations[k] != null).ToList();
    }

    public Unit? UnitAt(GridPosition gp) {
        return reservations[gp] ?? map[gp];
    }

    // accessible area
    // move a unit into a gridPosition, transform and all. ONly if not reserved
    public void MoveUnit(Unit unit, GridPosition gp) {
        if (map[gp] == null && (reservations[gp] == null || reservations[gp] == unit)) {
            GridPosition prevGridPosition = unit.gridPosition;
            Debug.Log($"Captured prev position {prevGridPosition}");
            AlignUnit(unit, gp);

            map[unit.gridPosition] = unit;
            Debug.Log($"Inserted {unit} into {unit.gridPosition}");
            if (prevGridPosition != null) {
                Debug.Log($"Setting {prevGridPosition} to null");
                map[prevGridPosition] = null;
            }

            if (reservations[gp] == unit) {
                reservations[gp] = null;
            }
        } else {
            Debug.Log($"Failed to move {unit} into occupied GP {gp}");
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
        
    private void AlignUnit(Unit unit, GridPosition gp) {
        unit.gridPosition = gp;
        unit.transform.position = battleMap.GridToWorld(gp);
    }
}
