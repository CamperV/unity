using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Extensions;

// State tree can only work in this way:
//     Idle
//     PlayerUnit.IssueCommand()) -> CommandActive
//     activeCommand.ActiveInteractAt -> CommandInProgress
//     CommandInProgress -> activeCommand.FinishCommand -> Idle
[RequireComponent(typeof(PlayerUnit))]
public class UnitCommandSystem : MonoBehaviour, IStateMachine<UnitCommandSystem.State>
{
    // IStateMachine<>
    public enum State {
        Idle,               // idle, obv
        CommandActive,      // Command has been selected, and is awaiting player input (a la MoveSelection or AttackSelection)
        CommandInProgress   // Command has received input, and is waiting on something (a la Moving/Attacking/Animation resolving)        
    }
    public State state { get; set; } = State.Idle;

    [SerializeField] private List<UnitCommand> unitCommands; // assigned in inspector
    [SerializeField] private UnitCommand DefaultCommand => unitCommands[0];
    private Dictionary<string, bool> commandAvailable = new Dictionary<string, bool>();

    // these are paired together to maintain state
    // inject this flag into activeCommand communications
    [SerializeField] private UnitCommand activeCommand { get; set; } = null;
    [SerializeField] private bool auxiliaryInteractFlag = false;

    private PlayerUnit thisUnit;

    void Awake() {
        thisUnit = GetComponent<PlayerUnit>();

        foreach (UnitCommand uc in unitCommands) {
            commandAvailable[uc.name] = true;
        }
    }

    void Update() {
        switch(state) {
            case State.Idle:
                break;
            
            case State.CommandActive:
                activeCommand.ActiveUpdate(thisUnit);
                break;

            case State.CommandInProgress:
                UnitCommand.ExitSignal changeState = activeCommand.InProgressUpdate(thisUnit);
                if (changeState == UnitCommand.ExitSignal.NextState) {
                    ChangeState(State.Idle);
                }
                break;
        }
    }

    // IStateMachine<>
    public void ChangeState(State newState) {
        if (newState == state) return;
        
        ExitState(state);
        EnterState(newState);
    }

    // IStateMachine<>
    public void InitialState() {
        ExitState(state);
        EnterState(State.Idle);
    }

    // IStateMachine<>
    public void EnterState(State enteringState) {
        state = enteringState;

        switch(state) {
            case State.Idle:
                activeCommand = null;
                auxiliaryInteractFlag = false;
                break;

            case State.CommandActive:
                activeCommand.Activate(thisUnit);
                thisUnit.personalAudioFX.PlayInteractFX();
                break;

            case State.CommandInProgress:
                break;
        }
    }

    // IStateMachine<>
    public void ExitState(State exitingState) {
        state = State.Idle;

        switch(exitingState) {
            case State.Idle:
                break;

            case State.CommandActive:
                activeCommand.Deactivate(thisUnit);
                break;

            case State.CommandInProgress:
                UnitCommand.ExitSignal exitSignal = activeCommand.FinishCommand(thisUnit, auxiliaryInteractFlag);

                switch (exitSignal) {
                    case UnitCommand.ExitSignal.ContinueTurn:
                        break;

                    case UnitCommand.ExitSignal.ForceFinishTurn:
                        thisUnit.FinishTurn();
                        break;
                }

                // either way, you can't use it again
                commandAvailable[activeCommand.name] = false;
                if (GetAvailableCommandCount() == 0) thisUnit.FinishTurn();
                break;
        }
    }

    // this can happen from a user clicking on a button, or PlayerUnit calling it directly by default (ie MoveUC)
    public void TryIssueCommand(UnitCommand command) {
        if (commandAvailable[command.name]) {
            activeCommand = command;
            ChangeState(State.CommandActive);
        }
    }

    public void CancelActiveCommand() {
        ChangeState(State.Idle);    // this puts activeCommand to null
    }

    public void Interact(GridPosition interactAt, bool auxiliaryInteract) {
        auxiliaryInteractFlag = auxiliaryInteract;

        switch(state) {
            case State.Idle:
                if (interactAt == thisUnit.gridPosition) {
                    if (auxiliaryInteract) {
                        TryIssueCommand(unitCommands[1]);
                    } else {
                        TryIssueCommand(DefaultCommand);
                    }
                }
                break;

            case State.CommandActive:
                UnitCommand.ExitSignal changeState = activeCommand.ActiveInteractAt(thisUnit, interactAt, auxiliaryInteract);
                if (changeState == UnitCommand.ExitSignal.NextState) {
                    ChangeState(State.CommandInProgress);
                }
                break;

            case State.CommandInProgress:
                break;
        }
    }

    public void SetAllCommandsAvailability(bool val) {
        foreach (string commandName in commandAvailable.Keys) {
            commandAvailable[commandName] = val;
        }
    }

    private int GetAvailableCommandCount() {
        int numAvailable = 0;
        foreach (bool ava in commandAvailable.Values) {
            if (ava) numAvailable++;
        }
        return numAvailable;
    }
}
