using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UnitMap : MonoBehaviour
{
    private Dictionary<GridPosition, Unit> unitMap;
    private BattleMap battleMap;

    void Awake() {
        unitMap = new Dictionary<GridPosition, Unit>();
        battleMap = GetComponentInChildren<BattleMap>();
    }

    void Start() {
        // init map to have no blanks
        foreach (GridPosition gp in battleMap.Positions) {
            unitMap[gp] = null;
        }

        // find all active objects that have a gridPosition
        // this happens only for Entities already in the hierarchy
        foreach (Unit unit in GetComponentsInChildren<Unit>()) {
            GridPosition startingGP = battleMap.ClosestGridPosition(unit.transform.position);
            MoveUnit(unit, startingGP);
        }
    }
    
    private void AlignUnit(Unit unit, GridPosition gp) {
        unit.gridPosition = gp;
        unit.transform.position = battleMap.GridToWorld(gp);
    }

    // accessible area
    public void MoveUnit(Unit unit, GridPosition gp) {
        if (unitMap[gp] == null) {
            GridPosition prevGridPosition = unit.gridPosition;
            AlignUnit(unit, gp);

            unitMap[unit.gridPosition] = unit;
            if (prevGridPosition != null) unitMap[prevGridPosition] = null;
        } else {
            Debug.Log($"Failed to move {unit} into occupied GP {gp}");
        }
    }
}
