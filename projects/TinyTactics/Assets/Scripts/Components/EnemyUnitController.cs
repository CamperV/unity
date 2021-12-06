using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class EnemyUnitController : MonoBehaviour, IStateMachine<EnemyUnitController.ControllerFSM>
{
    [SerializeField] public List<EnemyUnit> entities;

    public enum ControllerFSM {
        Inactive,
        Active
    }
    [SerializeField] private ControllerFSM state = ControllerFSM.Inactive;

    void Start() {
        // this accounts for all in-scene Entities, not instatiated prefabs
        foreach (EnemyUnit en in GetComponentsInChildren<EnemyUnit>()) {
            entities.Add(en);
        }

        EnterState(ControllerFSM.Active);
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
            case ControllerFSM.Active:
                break;
        }
        state = ControllerFSM.Inactive;
    }

    public void EnterState(ControllerFSM enteringState) {
        Debug.Log($"{this} entering state {enteringState}");
        state = enteringState;

        switch (state) {
            case ControllerFSM.Inactive:
            case ControllerFSM.Active:
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
            case ControllerFSM.Active:
                EnemyUnit? en = MatchingUnitAt(gp);
                if (en != null) {
                    Debug.Log($"Enemy {en} found at {gp}");
                }
                break;
        }
    }

    public EnemyUnit? MatchingUnitAt(GridPosition gp) {
        foreach (EnemyUnit en in entities) {
            if (en.gridPosition == gp) return en;
        }
        return null;
    }
}