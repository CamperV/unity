using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class PlayerUnitController : MonoBehaviour, IStateMachine<PlayerUnitController.ControllerFSM>
{
    [SerializeField] public List<PlayerUnit> entities;

    public enum ControllerFSM {
        Inactive,
        NoSelection,
        Selection
    }
    [SerializeField] private ControllerFSM state = ControllerFSM.Inactive;
    [SerializeField] private PlayerUnit currentSelection;


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
        Debug.Log($"{this} exiting state {exitingState}");

        switch (exitingState) {
            case ControllerFSM.Inactive:
                break;

            case ControllerFSM.NoSelection:
                break;

            case ControllerFSM.Selection:
                currentSelection = null;
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
                currentSelection = MatchingUnitAt(gp);

                if (currentSelection != null) {
                    currentSelection.ContextualInteractAt(gp);
                    ChangeState(ControllerFSM.Selection);
                }
                break;

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // There are two things that can happen here:                                                             //
            //      1) If you click on a different unit, de-select current and select the new                         //
            //      2) If you don't, currentSelection will polymorphically decide what it wants to do (via its state) //
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            case ControllerFSM.Selection:
                PlayerUnit? en = MatchingUnitAt(gp);

                // if you click on another Unit while in Selection of another, switch
                if (en != null && en != currentSelection) {
                    currentSelection?.ChangeState(PlayerUnit.PlayerUnitFSM.Idle);
                    currentSelection = en;
                }

                // regardless of whomever the currentSelection is, have them fire ContextualInteractAt
                currentSelection.ContextualInteractAt(gp);
                break;
        }
    }

    public void ContextualNoInteract() {
        switch (state) {
            case ControllerFSM.Inactive:
            case ControllerFSM.NoSelection:
                break;
            case ControllerFSM.Selection:
                // if (currentSelection)
                break;
        }
    }

    public void ClearInteraction() {
        currentSelection?.ChangeState(PlayerUnit.PlayerUnitFSM.Idle);
        ChangeState(ControllerFSM.NoSelection);
        // leaving Selection stage will reset currentSelection=null
    }

    public PlayerUnit? MatchingUnitAt(GridPosition gp) {
        foreach (PlayerUnit en in entities) {
            if (en.gridPosition == gp) return en;
        }
        return null;
    }
}