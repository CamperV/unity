using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUnitController : MonoBehaviour, IStateMachine<PlayerUnitController.ControllerFSM>
{
    // publicly acccessible events
    public delegate void UnitSelection(PlayerUnit selection);
    public event UnitSelection NewPlayerUnitControllerSelection;
    public event UnitSelection ClearPlayerUnitControllerSelection;

    // this is a flag that allows PlayerUnitController to be locked, ie can't ClearSelection()
    // this is done by certain unit actions, so that you don't switch units when trying to heal them
    public bool selectionLocked = false;

    [SerializeField] private List<PlayerUnit> _activeUnits;
    public List<PlayerUnit> activeUnits {
        get => _activeUnits.Where(en => en.gameObject.activeInHierarchy).ToList();
    }
    public List<PlayerUnit> disabledUnits => _activeUnits.Where(en => !en.gameObject.activeInHierarchy).ToList();

    public enum ControllerFSM {
        Inactive,
        NoSelection,
        Selection
    }
    [SerializeField] public ControllerFSM state { get; set; } = ControllerFSM.Inactive;

    private PlayerUnit currentSelection;
    private PlayerUnit mostRecentlySelectedUnit;
    private EnemyUnitController enemyUnitController;

    void Awake() {
        Battle _topBattleRef = GetComponentInParent<Battle>();
        enemyUnitController = _topBattleRef.GetComponentInChildren<EnemyUnitController>();
    }

    void Start() {
        // this accounts for all in-scene activeUnits, not instatiated prefabs
        // useful for Demo content
        foreach (PlayerUnit en in GetComponentsInChildren<PlayerUnit>()) {
            if (Campaign.active == null) {
                _activeUnits.Add(en);
            } else {
                Destroy(en.gameObject);
            }
        }

        // if you're not in an active Campaign, ie Demo, destroy all spawnMarkers
        if (Campaign.active == null) {
            foreach (SpawnMarker sm in GetComponentsInChildren<SpawnMarker>()) {
                Destroy(sm.gameObject);
            }
        }

        InitialState();
    }

    void Update() {
        switch (state) {
            case ControllerFSM.Inactive:
            case ControllerFSM.NoSelection:
                break;

            ////////////////////////////////////////////////////////////////////////
            // as soon as your currentSelection finishes their turn, change state //
            ////////////////////////////////////////////////////////////////////////
            case ControllerFSM.Selection:
                if (currentSelection.turnActive == false) ClearSelection();
                break;
        }
    }

    public void Lock() => selectionLocked = true;
    public void Unlock() => selectionLocked = false;

    public void ChangeState(ControllerFSM newState) {
        ExitState(state);
        EnterState(newState);
    }

    public void InitialState() {
        ExitState(state);
        EnterState(ControllerFSM.Inactive);
    }

    public void ExitState(ControllerFSM exitingState) {
        switch (exitingState) {
            case ControllerFSM.Inactive:
            case ControllerFSM.NoSelection:
                break;

            case ControllerFSM.Selection:
                // re-enable EnemyUnitController
                enemyUnitController.ChangeState(EnemyUnitController.ControllerFSM.NoPreview);
                break;
        }
        state = ControllerFSM.Inactive;
    }

    public void EnterState(ControllerFSM enteringState) {
        state = enteringState;
    
        switch (state) {
            case ControllerFSM.Inactive:
            case ControllerFSM.NoSelection:
                break;

            case ControllerFSM.Selection:
                // disable enemy unit controller for a time
                enemyUnitController.ChangeState(EnemyUnitController.ControllerFSM.Inactive);
                break;
        }
    }

    public void RegisterUnit(PlayerUnit unit) => _activeUnits.Add(unit);

    public void TriggerPhase() {
        // re-focus the camera on the centroid of your units
        // Vector3[] unitPositions = activeUnits.Select(u => u.transform.position).ToArray();
        // CameraManager.FocusActiveCameraOn( VectorUtils.Centroid(unitPositions) );

        // disable enemy unit controller for a time
        enemyUnitController.ChangeState(EnemyUnitController.ControllerFSM.NoPreview);
        ChangeState(ControllerFSM.NoSelection);

        activeUnits.ForEach(it => it.StartTurn());
    }

    // we refresh at the end of the phase,
    // because we want color when it isn't your turn,
    // and because it's possible the other team could add statuses that 
    public void EndPhase() {
        ChangeState(ControllerFSM.Inactive);

        // if you end the phase, and you never selected anyone, choose the first just so the camera refocuses
        if (mostRecentlySelectedUnit == null) mostRecentlySelectedUnit = activeUnits[0];
    }

    public void RefreshUnits() => activeUnits.ForEach(it => it.RefreshInfo());

    public void ContextualInteractAt(GridPosition gp, bool auxiliaryInteract) {
        switch (state) {
            /////////////////////////////////////////////////////
            // When the Controller is inactive, we do nothing. //
            /////////////////////////////////////////////////////
            case ControllerFSM.Inactive:
                break;

            /////////////////////////////////////////////////////////////////////////////////
            // When the player interacts with the grid while there is no active selection, //
            // we attempt to make a selection.                                             //
            /////////////////////////////////////////////////////////////////////////////////
            case ControllerFSM.NoSelection:
                SetCurrentSelection( MatchingUnitAt(gp) );
                currentSelection?.OnInteract(gp, auxiliaryInteract);
                break;

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // There are two things that can happen here:                                                             //
            //      1) If you click on a different unit, de-select current and select the new                         //
            //      2) If you don't, currentSelection will polymorphically decide what it wants to do (via its state) //
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            case ControllerFSM.Selection:
                if (!selectionLocked) {
                    PlayerUnit unit = MatchingUnitAt(gp);

                    // swap to the new unit. This will rapidly drop currentSelection (via Cancel/ChangeState(Idle))
                    // then REACQUIRE a currentSelection immediately afterwards
                    if (unit != null && unit != currentSelection) {
                        ClearSelection();
                        SetCurrentSelection(unit);
                    }
                }

                currentSelection.OnInteract(gp, auxiliaryInteract);
                break;
        }
    }

    public void SetCurrentSelection(PlayerUnit selection) {
        currentSelection = selection;

        if (selection == null) {
            ChangeState(ControllerFSM.NoSelection);
        } else {
            mostRecentlySelectedUnit = selection;
            ClearPlayerUnitControllerSelection?.Invoke(mostRecentlySelectedUnit);

            ChangeState(ControllerFSM.Selection);
        }

        NewPlayerUnitControllerSelection?.Invoke(selection);
    }

    public void ClearSelection() {
        if (state == ControllerFSM.Selection) {
            if (currentSelection.turnActive) currentSelection.RevertTurn();
            SetCurrentSelection(null);
        }
    }

    public PlayerUnit MatchingUnitAt(GridPosition gp) {
        foreach (PlayerUnit en in activeUnits) {
            if (en.gridPosition == gp) return en;
        }
        return null;
    }

    public void CheckEndPhase() {
        // check every time a unit finishes a turn
        // why don't we do this only on contextualActions?
        // because of the "Attacking" state. We must wait until animations are over
        bool endPlayerPhase = true;
        foreach (PlayerUnit unit in activeUnits) {
            endPlayerPhase &= !unit.turnActive;
        }
        
        if (endPlayerPhase) _EndPlayerPhase();
    }

    public void ForceEndPlayerPhase() {
        if (state != ControllerFSM.Inactive) {
            foreach (PlayerUnit u in activeUnits) {
                if (u.turnActive) u.WaitNoCheck();
            }
            _EndPlayerPhase();
        }
    }

    private void _EndPlayerPhase() {
        SetCurrentSelection(null);
        GetComponentInParent<TurnManager>().playerPhase.TriggerEnd();
    }
}