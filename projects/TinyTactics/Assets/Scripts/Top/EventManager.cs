using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public sealed class EventManager : MonoBehaviour
{
    public static EventManager inst = null; // enforces singleton behavior

    public Battle topBattleRef;
    public PlayerInputController inputController;
    public UIManager uiManager;
    public TurnManager turnManager;
    public UnitMap unitMap;
    public BattleMap battleMap;
    public PlayerUnitController playerUnitController;
    public EnemyUnitController enemyUnitController;

    void Awake() {
        // only allow one EventManager to exist at any time
        // & don't kill when reloading a Scene
        if (inst == null) {
            inst = this;
        } else if (inst != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start() {
        // input events
        inputController.MousePositionEvent += battleMap.CheckMouseOver;
        
        inputController.LeftMouseClickEvent += battleMap.CheckLeftMouseClick;

        inputController.RightMouseClickEvent += battleMap.CheckRightMouseClick;
        inputController.RightMouseClickEvent += _ => playerUnitController.ClearSelection();
        inputController.RightMouseClickEvent += _ => enemyUnitController.ClearPreview();

        inputController.LeftMouseHoldStartEvent += battleMap.CheckLeftMouseHoldStart;
        inputController.LeftMouseHoldEndEvent += battleMap.CheckLeftMouseHoldEnd;

        inputController.MainInteractButtonEvent += playerUnitController.ForceEndPlayerPhase;

        // battlemap events
        battleMap.InteractEvent += playerUnitController.ContextualInteractAt;
        battleMap.InteractEvent += enemyUnitController.ContextualInteractAt;

        battleMap.AuxiliaryInteractEvent_0 += playerUnitController.StartHoldTimer;
        battleMap.AuxiliaryInteractEvent_1 += playerUnitController.EndHoldTimer;

        // turn management events
        turnManager.playerPhase.StartEvent += playerUnitController.TriggerPhase;
        turnManager.playerPhase.EndEvent += playerUnitController.EndPhase;
        turnManager.enemyPhase.StartEvent += enemyUnitController.TriggerPhase;
        turnManager.enemyPhase.EndEvent += enemyUnitController.EndPhase;

        turnManager.NewTurnEvent += uiManager.UpdateTurn;
        turnManager.NewPhaseEvent += uiManager.UpdatePhase;
        turnManager.NewPhaseEvent += _ => playerUnitController.RefreshUnits();
        turnManager.NewPhaseEvent += _ => enemyUnitController.RefreshUnits();

        // board state events
        unitMap.NewBoardStateEvent += () => playerUnitController.activeUnits.ForEach(en => en.UpdateThreatRange());
        unitMap.NewBoardStateEvent += () => enemyUnitController.activeUnits.ForEach(en => en.UpdateThreatRange());

        // all board state update events should make TopBattle check if it should end now
        unitMap.NewBoardStateEvent += topBattleRef.CheckVictoryConditions;
        unitMap.NewBoardStateEvent += topBattleRef.CheckDefeatConditions;
    }
}