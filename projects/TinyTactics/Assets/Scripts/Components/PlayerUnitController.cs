using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUnitController : MonoBehaviour, IStateMachine<PlayerUnitController.ControllerFSM>
{
    // publicly acccessible events
    public delegate void UnitSelection(Unit selection);
    public event UnitSelection NewPlayerUnitControllerSelection;

    // debug
    public Text debugStateLabel;

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
    private EnemyUnitController enemyUnitController;

    void Awake() {
        Battle _topBattleRef = GetComponentInParent<Battle>();
        enemyUnitController = _topBattleRef.GetComponentInChildren<EnemyUnitController>();
    }

    void Start() {
        // this accounts for all in-scene activeUnits, not instatiated prefabs
        foreach (PlayerUnit en in GetComponentsInChildren<PlayerUnit>()) {
            _activeUnits.Add(en);
        }

        InitialState();
    }

    void Update() {
        ContextualNoInteract();
    }

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
    
        // debug
        debugStateLabel.text = $"PlayerUnitController: {state.ToString()}";

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

    public void TriggerPhase() {
        // disable enemy unit controller for a time
        enemyUnitController.ChangeState(EnemyUnitController.ControllerFSM.NoPreview);
        ChangeState(ControllerFSM.NoSelection);

        activeUnits.ForEach(it => it.StartTurn() );
    }

    // we refresh at the end of the phase,
    // because we want color when it isn't your turn,
    // and because it's possible the other team could add statuses that 
    // disable attackAvailable/moveAvailable etc
    public void EndPhase() {
        ChangeState(ControllerFSM.Inactive);
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
                currentSelection?.ContextualInteractAt(gp, auxiliaryInteract);
                break;

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // There are two things that can happen here:                                                             //
            //      1) If you click on a different unit, de-select current and select the new                         //
            //      2) If you don't, currentSelection will polymorphically decide what it wants to do (via its state) //
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            case ControllerFSM.Selection:
                PlayerUnit? unit = MatchingUnitAt(gp);

                // swap to the new unit. This will rapidly drop currentSelection (via Cancel/ChangeState(Idle))
                // then REACQUIRE a currentSelection immediately afterwards
                if (unit != null && unit != currentSelection) {
                    ClearSelection();
                    SetCurrentSelection(unit);
                }

                currentSelection.ContextualInteractAt(gp, auxiliaryInteract);
                break;
        }
    }

    public void ContextualNoInteract() {
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

    public void SetCurrentSelection(PlayerUnit selection) {
        currentSelection = selection;

        if (selection == null) {
            ChangeState(ControllerFSM.NoSelection);
        } else {
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

    public PlayerUnit? MatchingUnitAt(GridPosition gp) {
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
                u.WaitNoCheck();
            }
            _EndPlayerPhase();
        }
    }

    private void _EndPlayerPhase() => GetComponentInParent<TurnManager>().playerPhase.TriggerEnd();

    // this gets called when the BattleMap has a tile that a MouseHold event has triggered over
    private PlayerUnit _holdUnit;

    public void CheckWaitAt(GridPosition gp) {
        PlayerUnit? unit = MatchingUnitAt(gp);
        unit?.ContextualWait();
        _holdUnit = unit;
    }

    public void CancelWait(GridPosition gp) {
        _holdUnit?.CancelWait();
    }
}