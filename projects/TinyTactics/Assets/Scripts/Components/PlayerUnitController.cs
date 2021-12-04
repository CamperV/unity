using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class PlayerUnitController : MonoBehaviour, IStateMachine<PlayerUnitController.ControllerFSM>
{
    [SerializeField] public List<GridEntity> entities;

    public enum ControllerFSM {
        Inactive,
        NoSelection,
        Selection
    }
    [SerializeField] private ControllerFSM state = ControllerFSM.Inactive;
    [SerializeField] private GridEntity currentSelection;


    void Start() {
        // this accounts for all in-scene Entities, not instatiated prefabs
        foreach (GridEntity en in GetComponentsInChildren<GridEntity>()) {
            entities.Add(en);
        }

        EnterState(ControllerFSM.NoSelection);
    }

    public void ChangeState(ControllerFSM newState) {
        ExitState(state);
        EnterState(newState);
    }

    public void ExitState(ControllerFSM exitingState) {
        Debug.Log($"{this} exiting state {exitingState}");

        switch (exitingState) {
            case ControllerFSM.Inactive:
            case ControllerFSM.NoSelection:
            case ControllerFSM.Selection:
                break;
        }
        state = ControllerFSM.Inactive;
    }

    public void EnterState(ControllerFSM enteringState) {
        Debug.Log($"{this} entering state {enteringState}");
        state = enteringState;

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
                currentSelection = MatchingEntityAt(gp);

                if (currentSelection != null) {
                    currentSelection.ContextualInteractAt(gp);
                    ChangeState(ControllerFSM.Selection);
                }
                break;

            case ControllerFSM.Selection:
                GridEntity? en = MatchingEntityAt(gp);

                // if you click on another Entity while in Selection of another, switch
                if (en != null) {
                    currentSelection?.ChangeState(GridEntity.GridEntityFSM.Idle);
                    currentSelection = en;
                }

                // regardless of whomever the currentSelection is, have them fire ContextualInteractAt
                currentSelection.ContextualInteractAt(gp);
                break;
        }
    }

    public void ClearInteraction() {
        currentSelection?.ChangeState(GridEntity.GridEntityFSM.Idle);
        ChangeState(ControllerFSM.NoSelection);
    }

    public GridEntity? MatchingEntityAt(GridPosition gp) {
        foreach (GridEntity en in entities) {
            if (en.gridPosition == gp) return en;
        }
        return null;
    }
}