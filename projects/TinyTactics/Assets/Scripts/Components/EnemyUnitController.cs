using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUnitController : MonoBehaviour, IStateMachine<EnemyUnitController.ControllerFSM>
{
    // debug
    public Text debugStateLabel;

    [SerializeField] public List<EnemyUnit> entities;

    public enum ControllerFSM {
        Inactive,
        NoSelection,
        Selection
    }
    [SerializeField] public ControllerFSM state { get; set; } = ControllerFSM.Inactive;

    private EnemyUnit _currentSelection;
    private EnemyUnit currentSelection {
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

    void Start() {
        // this accounts for all in-scene Entities, not instatiated prefabs
        foreach (EnemyUnit en in GetComponentsInChildren<EnemyUnit>()) {
            entities.Add(en);
        }

        EnterState(ControllerFSM.NoSelection);
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
        debugStateLabel.text = $"EnemyUnitController: {state.ToString()}";

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

            case ControllerFSM.Selection:
                EnemyUnit? unit = MatchingUnitAt(gp);

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

    public void ClearSelection() {
        currentSelection = null;
    }

    public EnemyUnit? MatchingUnitAt(GridPosition gp) {
        foreach (EnemyUnit en in entities) {
            if (en.gridPosition == gp) return en;
        }
        return null;
    }
}