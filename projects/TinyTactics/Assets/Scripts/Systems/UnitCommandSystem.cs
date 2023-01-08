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
public class UnitCommandSystem : MonoBehaviour, IStateMachine<UnitCommandSystem.State>
{
    // this is only set inside this file. Convenience, so I don't have to re-add Move, Attack, Wait, etc over and over
    public static string defaultUCP = "ScriptableObjects/UnitCommands/DefaultUnitCommandPool";

	public delegate void UnitCommandStateChange(PlayerUnit thisUnit, UnitCommand uc);
    public event UnitCommandStateChange ActivateUC;
    public event UnitCommandStateChange DeactivateUC;
    public event UnitCommandStateChange FinishUC;
    public event UnitCommandStateChange RevertUC;

    // IStateMachine<>
    public enum State {
        Idle,               // idle, obv
        CommandActive,      // Command has been selected, and is awaiting player input (a la MoveSelection or AttackSelection)
        CommandInProgress   // Command has received input, and is waiting on something (a la Moving/Attacking/Animation resolving)        
    }
    [field: SerializeField] public State state { get; set; } = State.Idle;

    [SerializeField] private List<UnitCommand> unitCommands;
    public IEnumerable<UnitCommand> Commands => unitCommands;
    private Dictionary<UnitCommand.CommandCategory, UnitCommand> categoryDefaults;

    // "can I currently use this command", not necessarily dependent on usage
    private Dictionary<string, bool> commandAvailable = new Dictionary<string, bool>();

    // "cooldown until you can use again"
    // shadow: restoring cooldowns for reverting actions
    private Dictionary<string, int> commandCooldowns = new Dictionary<string, int>();
    private Dictionary<string, int> _shadowCooldowns = new Dictionary<string, int>();

    // "how many discrete usages are left"
    // shadow: restoring uses for reverting actions
    private Dictionary<string, int> commandUses = new Dictionary<string, int>();
    private Dictionary<string, int> _shadowUses = new Dictionary<string, int>();
    //
    public bool IsCommandAvailable(UnitCommand uc) {
        return commandAvailable[uc.name] && IsAvailableLimitType(uc) && uc.IsAvailableAux(boundUnit);
    }
    public int CommandCooldown(UnitCommand uc) => commandCooldowns[uc.name];
    public int CommandRemainingUses(UnitCommand uc) => commandUses[uc.name];

    private bool IsAvailableLimitType(UnitCommand uc) {
        switch (uc.limitType) {
            case UnitCommand.LimitType.Unlimited:
                return true;
            case UnitCommand.LimitType.Cooldown:
                return commandCooldowns[uc.name] == 0;
            case UnitCommand.LimitType.LimitedUse:
                return commandUses[uc.name] > 0;
            default:
                Debug.LogError($"UC {uc} of limitType {uc.limitType} cannot be used.");
                return false;
        }
    }

    // these are paired together to maintain state
    // inject this flag into activeCommand communications
    [SerializeField] private UnitCommand activeCommand { get; set; } = null;
    [SerializeField] private bool auxiliaryInteractFlag = false;

    private PlayerUnit boundUnit;
    private List<UnitCommand> executedStack;

    void Awake() {
        boundUnit = GetComponent<PlayerUnit>();
        executedStack = new List<UnitCommand>();
        categoryDefaults = new Dictionary<UnitCommand.CommandCategory, UnitCommand>();

        // load in the defaults here in unitCommands
        // No way to add custom commands. Do that via MutationSystem
        UnitCommandPool defaultPool = Resources.Load<UnitCommandPool>(defaultUCP) as UnitCommandPool;

        // MUST iterate here, cannot do "unitCommands = defaultPool.unitCommands;"
        // if you do that, unitCommands becomes a reference to a ScriptableObject field, meaning you
        // lose instance-level information for unitCommandSystems everywhere
        // ALSO don't use AddCommand, that function assumes you've already inserted default commands
        foreach (UnitCommand uc in defaultPool.unitCommands) {
            unitCommands.Add(uc);
            InitCommandUsageData(uc);
            categoryDefaults[uc.commandCategory] = uc;
        }

        if (unitCommands.Count == 0) Debug.LogError($"No commands set for {this}/{boundUnit}");
    }

    void Update() {
        switch(state) {
            case State.Idle:
                break;
            
            case State.CommandActive:
                activeCommand.ActiveUpdate(boundUnit);
                break;

            case State.CommandInProgress:
                UnitCommand.ExitSignal changeState = activeCommand.InProgressUpdate(boundUnit);

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
        executedStack.Clear();

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
                activeCommand.Activate(boundUnit);
                ActivateUC?.Invoke(boundUnit, activeCommand);
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
                activeCommand.Deactivate(boundUnit);
                DeactivateUC?.Invoke(boundUnit, activeCommand);
                break;

            case State.CommandInProgress:
                CompleteCommand(activeCommand);
                break;
        }
    }

    // find the closest command with the same category, and add it to that
    public void AddCommand(UnitCommand command) {
        // insert into "like" commands
        int mostRecent = 0;
        if (command.commandCategory == UnitCommand.CommandCategory.None) {
            mostRecent = unitCommands.Count;
        } else {
            for (int i = 0; i < unitCommands.Count; i++) {
                if (unitCommands[i].commandCategory == command.commandCategory)
                    mostRecent = i;
            }
        }

        // never insert after "wait", which is last
        int at = Mathf.Max(0, Mathf.Min(unitCommands.Count - 1, mostRecent + 1));
        unitCommands.Insert(at, command);

        InitCommandUsageData(command);

        // finally, check if you're replacing something
        if (command.replaceDefault != UnitCommand.CommandCategory.None) {
            RemoveCommand(categoryDefaults[command.replaceDefault]);
        }
    }

    public void RemoveCommand(UnitCommand command) {
        unitCommands.Remove(command);
        //
        commandAvailable.Remove(command.name);
        //
        if (commandCooldowns.ContainsKey(command.name)) commandCooldowns.Remove(command.name);
        if (_shadowCooldowns.ContainsKey(command.name)) _shadowCooldowns.Remove(command.name);
        //
        if (commandUses.ContainsKey(command.name)) commandUses.Remove(command.name);
        if (_shadowUses.ContainsKey(command.name)) _shadowUses.Remove(command.name);

        // final check
        // this command has already replaced a default
        // so after removing it, restore the default
        if (command.replaceDefault != UnitCommand.CommandCategory.None) {
            AddCommand(categoryDefaults[command.replaceDefault]);
        }
    }

    // this can happen from a user clicking on a button, or PlayerUnit calling it directly by default (ie MoveUC)
    public void TryIssueCommand(UnitCommand command) {
        Debug.Log($"Trying to issue command {command}");
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

    public void RevertExecutedCommands() {
        foreach (UnitCommand uc in executedStack) {
            uc.Revert(boundUnit);
            //
            commandAvailable[uc.name] = true;

            switch (uc.limitType) {
                case UnitCommand.LimitType.Cooldown:
                    commandCooldowns[uc.name] = _shadowCooldowns[uc.name];
                    break;
                case UnitCommand.LimitType.LimitedUse:
                    commandUses[uc.name] = _shadowUses[uc.name];
                    break;
            }
            //
            RevertUC?.Invoke(boundUnit, uc);
        }
    }

    public void CompleteCommand(UnitCommand command) {
        UnitCommand.ExitSignal exitSignal = command.FinishCommand(boundUnit, auxiliaryInteractFlag);
        UseCommandLimitType(command);

        // go ahead and finish all commands like this one
        // this will insert the current command into the executedStack
        DisableSimilarCommands(command.commandCategory);

        switch (exitSignal) {
            // if this Command doesn't end your turn, you might be able to revert it
            // this is the case with MoveUC
            case UnitCommand.ExitSignal.ContinueTurn:
                // --> this is already performed above in DisableSimilarCommands()
                // executedStack.Insert(0, command);
                break;

            case UnitCommand.ExitSignal.ForceFinishTurn:
                boundUnit.FinishTurn();
                break;
        }
        
        if (GetAvailableCommandCount() == 0) boundUnit.FinishTurn();
    }

    public void Interact(GridPosition interactAt, bool auxiliaryInteract) {
        auxiliaryInteractFlag = auxiliaryInteract;

        switch(state) {
            case State.Idle:
                if (interactAt == boundUnit.gridPosition) {
                    TryIssueCommand( NextAvailableCommand(UnitCommand.ExecutionType.Interactive) );
                    
                    // wake-up sound
                    boundUnit.personalAudioFX.PlayWakeUpFX();
                }
                break;

            case State.CommandActive:
                UnitCommand.ExitSignal changeState = activeCommand.ActiveInteractAt(boundUnit, interactAt, auxiliaryInteract);
                if (changeState == UnitCommand.ExitSignal.NextState) {
                    ChangeState(State.CommandInProgress);
                }
                break;

            case State.CommandInProgress:
                break;
        }
    }

    public void TickCooldowns() {
        foreach (string commandName in commandCooldowns.Keys.ToList()) {
            int val = commandCooldowns[commandName];
            commandCooldowns[commandName] = Mathf.Max(0, val - 1);

            // store the start-of-turn value to revert back to
            _shadowCooldowns[commandName] = commandCooldowns[commandName];
        }
    }

    // if you're setting False, just do it
    // if you're setting True, you must also have a valid cooldown value
    public void SetAllCommandsAvailability(bool val) {
        foreach (UnitCommand command in unitCommands) {
            commandAvailable[command.name] = val && IsAvailableLimitType(command);
        }
    }

    public void DisableSimilarCommands(UnitCommand.CommandCategory commandCategory) {
        foreach (UnitCommand uc in unitCommands) {
            if (uc.commandCategory == commandCategory) {
                FinishUC?.Invoke(boundUnit, uc); // right now, this is hooked up to UI elements
                commandAvailable[uc.name] = false;

                // this is for reverting purposes
                executedStack.Insert(0, uc);
            }
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

    // do NOT update the _shadow versions. Those are used for reversion and can only be updated
    // on the starts or ends of turns
    private void UseCommandLimitType(UnitCommand command) {
        switch (command.limitType) {
            case UnitCommand.LimitType.Cooldown:
                commandCooldowns[command.name] = command.cooldown + 1;
                break;
            case UnitCommand.LimitType.LimitedUse:
                commandUses[command.name]--;
                break;
        }
    }

    private void InitCommandUsageData(UnitCommand command) {
        commandAvailable[command.name] = true;

        switch (command.limitType) {
            case UnitCommand.LimitType.Cooldown:
                commandCooldowns[command.name] = 0;
                _shadowCooldowns[command.name] = 0;
                break;

            case UnitCommand.LimitType.LimitedUse:
                commandUses[command.name] = command.remainingUses;
                _shadowUses[command.name] = command.remainingUses;
                break;
        }
    }
}
