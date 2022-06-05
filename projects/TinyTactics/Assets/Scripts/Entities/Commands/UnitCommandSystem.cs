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
	public delegate void UnitCommandStateChange(UnitCommand uc);
    public event UnitCommandStateChange ActivateUC;
    public event UnitCommandStateChange DeactivateUC;
    public event UnitCommandStateChange FinishUC;

    // IStateMachine<>
    public enum State {
        Idle,               // idle, obv
        CommandActive,      // Command has been selected, and is awaiting player input (a la MoveSelection or AttackSelection)
        CommandInProgress   // Command has received input, and is waiting on something (a la Moving/Attacking/Animation resolving)        
    }
    [field: SerializeField] public State state { get; set; } = State.Idle;

    [SerializeField] private List<UnitCommand> unitCommands; // assigned in inspector
    public IEnumerable<UnitCommand> Commands => unitCommands;

    private Dictionary<string, bool> commandAvailable = new Dictionary<string, bool>();
    public bool IsCommandAvailable(UnitCommand uc) => commandAvailable[uc.name] && uc.IsAvailableAux(thisUnit);

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

        if (unitCommands.Count == 0) Debug.LogError($"No commands set for {this}/{thisUnit}");
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

                // if we get the NextState signal, progress to clean-up, ie ExitState->FinishCommand
                if (changeState == UnitCommand.ExitSignal.NextState) {
                    ChangeState(State.Idle);

                    // then, if its available, try to go to next
                    // if this is null, TryIssueCommand will take care of it
                    TryIssueCommand( NextAvailableCommand(UnitCommand.ExecutionType.Interactive) );

                } else {
                    // we spin (ie ExitSignal.NoStateChange)
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
                ActivateUC?.Invoke(activeCommand);
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
                DeactivateUC?.Invoke(activeCommand);
                break;

            case State.CommandInProgress:
                CompleteCommand(activeCommand);
                break;
        }
    }

    // this can happen from a user clicking on a button, or PlayerUnit calling it directly by default (ie MoveUC)
    public void TryIssueCommand(UnitCommand command) {
        if (command == null) return;
        if (!IsCommandAvailable(command)) return;

        switch (command.executionType) {
            case UnitCommand.ExecutionType.Interactive:
                if (command != activeCommand) {
                    // go to Idle first, so that you exit from activeCommand properly
                    // otherwise, CommandActive -> CommandActive does nothing
                    ChangeState(State.Idle);
                    
                    //
                    activeCommand = command;
                    ChangeState(State.CommandActive);

                // else command == activeCommand, you're already active, deactivate it
                } else {
                    CancelActiveCommand();
                }
                break;

            case UnitCommand.ExecutionType.Immediate:
                ChangeState(State.Idle);
                CompleteCommand(command);
                break;
        }
    }

    public void CancelActiveCommand() {
        ChangeState(State.Idle);    // this puts activeCommand to null
    }

    public void CompleteCommand(UnitCommand command) {
        UnitCommand.ExitSignal exitSignal = command.FinishCommand(thisUnit, auxiliaryInteractFlag);
        FinishUC?.Invoke(command);
        commandAvailable[command.name] = false;

        switch (exitSignal) {
            case UnitCommand.ExitSignal.ContinueTurn:
                break;

            case UnitCommand.ExitSignal.ForceFinishTurn:
                thisUnit.FinishTurn();
                break;
        }
        
        if (GetAvailableCommandCount() == 0) thisUnit.FinishTurn();
    }

    public void Interact(GridPosition interactAt, bool auxiliaryInteract) {
        auxiliaryInteractFlag = auxiliaryInteract;

        switch(state) {
            case State.Idle:
                if (interactAt == thisUnit.gridPosition) TryIssueCommand( NextAvailableCommand(UnitCommand.ExecutionType.Interactive) );
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
        foreach (string commandName in commandAvailable.Keys.ToList()) {
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

    // this will return null for "Default"
    private UnitCommand NextAvailableCommand(UnitCommand.ExecutionType executionType) {
        return unitCommands.FirstOrDefault(uc => IsCommandAvailable(uc) && uc.executionType == executionType);
    }
}
