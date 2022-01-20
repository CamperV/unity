using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public sealed class EventManager : MonoBehaviour
{
    public Battle topBattleRef;
    public PlayerInputController inputController;
    public PlayerInputController menuInputController;
    public CameraManager cameraManager;
    public UIManager uiManager;
    public TurnManager turnManager;
    public UnitMap unitMap;
    public BattleMap battleMap;
    public PlayerUnitController playerUnitController;
    public EnemyUnitController enemyUnitController;

    public void EnablePlayerInput() => inputController.gameObject.SetActive(true);
    public void DisablePlayerInput() => inputController.gameObject.SetActive(false);
    //
    public void EnableMenuInput() => menuInputController.gameObject.SetActive(true);
    public void DisableMenuInput() => menuInputController.gameObject.SetActive(false);

    public void RegisterEvents() {
        // input events
        inputController.MousePositionEvent += battleMap.CheckMouseOver;
        
        inputController.LeftMouseClickEvent += battleMap.CheckLeftMouseClick;

        inputController.RightMouseClickEvent += battleMap.CheckRightMouseClick;
        inputController.RightMouseClickEvent += _ => playerUnitController.ClearSelection();
        inputController.RightMouseClickEvent += _ => enemyUnitController.ClearPreview();

        inputController.MiddleMouseClickEvent += battleMap.CheckMiddleMouseClick;

        inputController.LeftMouseHoldStartEvent += battleMap.CheckLeftMouseHoldStart;
        inputController.LeftMouseHoldEndEvent += battleMap.CheckLeftMouseHoldEnd;

        inputController.MainInteractButtonEvent += playerUnitController.ForceEndPlayerPhase;
        inputController.DirectionalInputEvent += cameraManager.UpdateMovementVector;

        // top!battle events
        topBattleRef.BattleStartEvent += turnManager.Enable;

        // battlemap events
        battleMap.InteractEvent += gp => playerUnitController.ContextualInteractAt(gp, false);
        battleMap.InteractEvent += enemyUnitController.ContextualInteractAt;

        battleMap.AuxiliaryInteractEvent_0 += playerUnitController.CheckWaitAt; // hold down
        battleMap.AuxiliaryInteractEvent_1 += playerUnitController.CancelWait;  // release
        battleMap.AuxiliaryInteractEvent_2 += gp => playerUnitController.ContextualInteractAt(gp, true);  // middle-click (special interact)

        battleMap.TerrainMouseOverEvent += uiManager.UpdateTerrainEffectPanel;

        // unit controller events
        playerUnitController.NewPlayerUnitControllerSelection += cameraManager.AcquireTrackingTarget;
        enemyUnitController.NewEnemyUnitControllerSelection += cameraManager.AcquireTrackingTarget;

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
        unitMap.NewBoardStateEvent += () => enemyUnitController.activeUnits.ForEach(en => en.RefreshTargets());
        // unitMap.NewBoardStateEvent += battleMap.ResetHighlight;

        // all board state update events should make TopBattle check if it should end now
        unitMap.NewBoardStateEvent += topBattleRef.CheckVictoryConditions;
        unitMap.NewBoardStateEvent += topBattleRef.CheckDefeatConditions;
    }
}