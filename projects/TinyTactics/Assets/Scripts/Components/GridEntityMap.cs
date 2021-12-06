using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridEntityMap : MonoBehaviour
{
    private Dictionary<GridPosition, GridEntity> entityMap;
    private BattleMap battleMap;

    [SerializeField] public List<GridEntity> entities;

    void Awake() {
        entityMap = new Dictionary<GridPosition, GridEntity>();
        battleMap = GetComponentInChildren<BattleMap>();
        entities = new List<GridEntity>();
    }

    void Start() {
        // init map to have no blanks
        foreach (GridPosition gp in battleMap.Positions) {
            entityMap[gp] = null;
        }

        // find all active objects that have a gridPosition
        // this happens only for Entities already in the hierarchy
        foreach (GridEntity en in GetComponentsInChildren<GridEntity>()) {
            GridPosition startingGP = battleMap.ClosestGridPosition(en.transform.position);
            MoveEntity(en, startingGP);

            entities.Add(en);
        }
    }
    
    private void AlignEntity(GridEntity en, GridPosition gp) {
        en.gridPosition = gp;
        en.transform.position = battleMap.GridToWorld(gp);
    }

    // accessible area
    public void MoveEntity(GridEntity en, GridPosition gp) {
        if (entityMap[gp] == null) {
            GridPosition prevGridPosition = en.gridPosition;
            AlignEntity(en, gp);

            entityMap[en.gridPosition] = en;
            if (prevGridPosition != null) entityMap[prevGridPosition] = null;
        } else {
            Debug.Log($"Failed to move {en} into occupied GP {gp}");
        }
    }

    public GridEntity? EntityAt(GridPosition gp) {
        return (entityMap.ContainsKey(gp)) ? entityMap[gp] : null;
    }
}
