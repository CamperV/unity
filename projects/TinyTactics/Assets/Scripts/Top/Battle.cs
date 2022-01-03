using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(UnitMap), typeof(TurnManager))]
public class Battle : MonoBehaviour
{
    public delegate void BattleEvent();
    public event BattleEvent BattleStartEvent;

    private EventManager eventManager;
    private UnitMap unitMap;
    private TurnManager turnManager;
    
    private PlayerUnitController playerUnitController;
    private EnemyUnitController enemyUnitController;

    void Awake() {
        eventManager = GetComponent<EventManager>();
        unitMap = GetComponent<UnitMap>();
        turnManager = GetComponent<TurnManager>();

        playerUnitController = GetComponentInChildren<PlayerUnitController>();
        enemyUnitController = GetComponentInChildren<EnemyUnitController>();
    }

    public void StartBattle() {
        eventManager.EnablePlayerInput();
        eventManager.RegisterEvents();
        //
        BattleStartEvent?.Invoke();
    }

    public void EndBattle(bool playerVictorious) {
        eventManager.DisablePlayerInput();

        int enemiesDefeated = enemyUnitController.disabledUnits.Count;
        int survivingUnits = playerUnitController.activeUnits.Count;
        int turnsElapsed = turnManager.turnCount;

        if (playerVictorious) {
            UIManager.inst.CreateVictoryPanel(enemiesDefeated, survivingUnits, turnsElapsed);
        } else {
            UIManager.inst.CreateDefeatPanel(enemiesDefeated, survivingUnits, turnsElapsed);
        }
    }

    public void CheckVictoryConditions() {
        // the main victory conditions is defeating all enemy units
        bool enemyUnitsAlive = enemyUnitController.activeUnits.Any();

        if (!enemyUnitsAlive) {
            UIManager.inst.combatLog.AddEntry($"GREEN@[VICTORY!]");
            EndBattle(true);
        }
    }

    public void CheckDefeatConditions() {
        // the main defeat condition is losing all of your units
        bool playerUnitsAlive = playerUnitController.activeUnits.Any();

        if (!playerUnitsAlive) {
            UIManager.inst.combatLog.AddEntry($"RED@[Y O U  D I E D]");
            EndBattle(false);
        }
    }
}
