using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(UnitMap), typeof(TurnManager))]
public class Battle : MonoBehaviour
{
    private UnitMap unitMap;
    private TurnManager turnManager;
    
    private PlayerUnitController playerUnitController;
    private EnemyUnitController enemyUnitController;

    void Awake() {
        unitMap = GetComponent<UnitMap>();
        turnManager = GetComponent<TurnManager>();

        playerUnitController = GetComponentInChildren<PlayerUnitController>();
        enemyUnitController = GetComponentInChildren<EnemyUnitController>();
    }

    public void StartBattle() {
        // turnManager enables automatically
        // all Units enter state automatically
        // all events are ordered correctly
    }

    public void CheckVictoryConditions() {
        // the main victory conditions is defeating all enemy units
        bool enemyUnitsAlive = enemyUnitController.activeUnits.Any();

        if (!enemyUnitsAlive) {
            Debug.Log($"=====================");
            Debug.Log($"====== VICTORY ======");
            Debug.Log($"=====================");
        }
    }

    public void CheckDefeatConditions() {
        // the main defeat condition is losing all of your units
        bool playerUnitsAlive = playerUnitController.activeUnits.Any();

        if (!playerUnitsAlive) {
            Debug.Log($"====================");
            Debug.Log($"====== DEFEAT ======");
            Debug.Log($"====================");
        }
    }
}
