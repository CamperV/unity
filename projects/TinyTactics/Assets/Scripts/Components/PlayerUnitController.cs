using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUnitController : MonoBehaviour, IStateMachine<PlayerUnitController.ControllerFSM>
{
    // debug
    public Text debugStateLabel;

    [SerializeField] public List<PlayerUnit> entities;

    public enum ControllerFSM {
        Inactive,
        NoSelection,
        Selection
    }
    [SerializeField] public ControllerFSM state { get; set; } = ControllerFSM.Inactive;

    private PlayerUnit _currentSelection;
    private PlayerUnit currentSelection {
        get => _currentSelection;
        set {
            _currentSelection = value;
            if (_currentSelection == null) {
                ChangeState(ControllerFSM.NoSelection);
            } else {
                ChangeState(ControllerFSM.Selection);
            }
        }
    }

    private TurnManager turnManager;

    void Awake() {
        turnManager = GetComponentInParent<TurnManager>();
    }

    void Start() {
        // this accounts for all in-scene Entities, not instatiated prefabs
        foreach (PlayerUnit en in GetComponentsInChildren<PlayerUnit>()) {
            entities.Add(en);
        }

        EnterState(ControllerFSM.NoSelection);
    }

    void Update() {
        ContextualNoInteract();
    }

    public void ChangeState(ControllerFSM newState) {
        ExitState(state);
        EnterState(newState);
    }

    public void ExitState(ControllerFSM exitingState) {
        switch (exitingState) {
            case ControllerFSM.Inactive:
            case ControllerFSM.NoSelection:
            case ControllerFSM.Selection:
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
            case ControllerFSM.Selection:
                break;
        }
    }

    public void ContextualInteractAt(GridPosition gp) {
        switch (state) {
            /////////////////////////////////////////////////////
            // When the Controller is inactive, we do nothing. //
            /////////////////////////////////////////////////////
            case ControllerFSM.Inactive:
                Debug.Log($"Sorry, I'm inactive");
                break;

            /////////////////////////////////////////////////////////////////////////////////
            // When the player interacts with the grid while there is no active selection, //
            // we attempt to make a selection.                                             //
            /////////////////////////////////////////////////////////////////////////////////
            case ControllerFSM.NoSelection:
                currentSelection = MatchingUnitAt(gp);
                currentSelection?.ContextualInteractAt(gp);
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
                    currentSelection.Cancel();
                    currentSelection = unit;
                }

                currentSelection.ContextualInteractAt(gp);
                break;
        }
    }

    public void ContextualNoInteract() {
        switch (state) {
            case ControllerFSM.Inactive:
                break;

            case ControllerFSM.NoSelection:
            case ControllerFSM.Selection:
                // TODO: SOON, CHANGE TO EVENT-BASED
                // IE, EACH UNIT SENDS AN EVENT WHEN IT IS FINISHED?
                //
                // every frame, check if we should end PlayerPhase or not
                // why don't we do this only on contextualActions?
                // because of the "Attacking" state. We must wait until animations are over
                bool endPlayerPhase = true;
                foreach (PlayerUnit unit in entities) {
                    endPlayerPhase &= !unit.turnActive;
                }
                
                if (endPlayerPhase) turnManager.playerPhase.TriggerEnd();
                break;
        }
    }

    public void ClearSelection() {
        currentSelection = null;
    }

    public PlayerUnit? MatchingUnitAt(GridPosition gp) {
        foreach (PlayerUnit en in entities) {
            if (en.gridPosition == gp) return en;
        }
        return null;
    }
}