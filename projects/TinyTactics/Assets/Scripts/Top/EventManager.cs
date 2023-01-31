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
    public PlayerInputController menuInputController;
    public CameraManager cameraManager;
    public UIManager uiManager;
    public TurnManager turnManager;
    public UnitMap unitMap;
    public BattleMap battleMap;
    public TerrainSystem terrainSystem;
    public UnitSelectionSystem unitSelectionSystem;
    public PlayerUnitController playerUnitController;
    public EnemyUnitController enemyUnitController;

    public GlobalMutationSystem playerGlobalMutationSystem;
	
    void Awake() {
        // only allow one EventManager to exist at any time
		// & don't kill when reloading a Scene
 		if (inst == null) {
			inst = this;
		} else if (inst != this) {
			Destroy(gameObject);
		}
    }

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
        inputController.RightMouseClickEvent += _ => unitSelectionSystem.ClearSelection();

        inputController.MiddleMouseClickEvent += battleMap.CheckAuxiliaryInteract;

        inputController.LeftMouseHoldStartEvent += battleMap.CheckLeftMouseHoldStart;
        inputController.LeftMouseHoldEndEvent += battleMap.CheckLeftMouseHoldEnd;

        inputController.MainInteractButtonEvent += playerUnitController.ForceEndPlayerPhase;
        inputController.SelectNextUnitEvent += unitSelectionSystem.SelectNextPlayerUnit;
        inputController.DirectionalInputEvent += cameraManager.UpdateMovementVector;
        // inputController.MouseScrollEvent += cameraManager.UpdateZoomLevel;
        // inputController.MouseScrollEvent += cameraManager.UpdateZoomLevel;

        // top!battle events
        topBattleRef.BattleStartEvent += turnManager.Enable;
        topBattleRef.BattleStartEvent += playerGlobalMutationSystem.Initialize;

        // battlemap events
        battleMap.InteractEvent += unitSelectionSystem.SelectAt;
        // battleMap.AuxiliaryInteractEvent_0 += // hold down
        // battleMap.AuxiliaryInteractEvent_1 += // release
        battleMap.AuxiliaryInteractEvent_2 += unitSelectionSystem.SelectAt_Aux;  // middle-click (special interact)

        battleMap.GridMouseOverEvent += terrainSystem.CheckTerrainMouseOver;

        // turn management events
        turnManager.playerPhase.StartEvent += playerUnitController.TriggerPhase;
        turnManager.playerPhase.EndEvent += playerUnitController.EndPhase;
        turnManager.enemyPhase.StartEvent += enemyUnitController.TriggerPhase;
        turnManager.enemyPhase.EndEvent += enemyUnitController.EndPhase;

        turnManager.NewPhaseEvent += _ => playerUnitController.RefreshUnits();
        turnManager.NewPhaseEvent += _ => enemyUnitController.RefreshUnits();

        turnManager.playerPhase.StartEvent += () => unitSelectionSystem.gameObject.SetActive(true);
        turnManager.playerPhase.EndEvent += () => unitSelectionSystem.gameObject.SetActive(false);

        // board state events
        unitMap.NewBoardStateEvent += () => playerUnitController.GetActiveUnits().ForEach(en => en.UpdateThreatRange());
        unitMap.NewBoardStateEvent += () => enemyUnitController.GetActiveUnits().ForEach(en => en.UpdateThreatRange());
        unitMap.NewBoardStateEvent += () => enemyUnitController.GetActiveUnits<EnemyUnit>().ForEach(en => en.RefreshTargets());

        // all board state update events should make TopBattle check if it should end now
        unitMap.NewBoardStateEvent += topBattleRef.CheckVictoryConditions;
        unitMap.NewBoardStateEvent += topBattleRef.CheckDefeatConditions;
    }
}
