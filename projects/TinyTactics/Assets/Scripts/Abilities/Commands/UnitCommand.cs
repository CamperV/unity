using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Extensions;
using UnityEngine.UI;

public abstract class UnitCommand : ScriptableObject
{
    public enum ExitSignal {
        // generally for Interactive commands
        NoStateChange,
        NextState,

        // generally for InProgress commands
        ContinueTurn,
        ForceFinishTurn
    }

    public new string name; // fillable via ScriptableObject interface
    public Sprite sprite; // fillable via ScriptableObject interface

    public enum ExecutionType {
        Interactive,
        Immediate
    }
    public ExecutionType executionType; // fillable via ScriptableObject interface

    public enum CommandCategory {
        Movement,
        Attack,
        Other
    }
    public CommandCategory commandCategory; // fillable via ScriptableObject interface

    public abstract void Activate(PlayerUnit thisUnit); /* Initial activation */
    public abstract void Deactivate(PlayerUnit thisUnit); /* De-activation, ie Cancel */
    public abstract ExitSignal ActiveInteractAt(PlayerUnit thisUnit, GridPosition interactAt, bool auxiliaryInteract); /* Interaction/execution while active */
    public abstract void ActiveUpdate(PlayerUnit thisUnit); /* Always-on while active */
    public abstract ExitSignal InProgressUpdate(PlayerUnit thisUnit); /* Always-on after executing */
    public abstract ExitSignal FinishCommand(PlayerUnit thisUnit, bool auxiliaryInteract); /* Finish, usually responsible for executing state changes like Unit Movement */

    // this is an optional method for specifying other ways a command can't be available.
    // For example, DefaultAttack can't be available unless there are targetable enemies in range
    public virtual bool IsAvailableAux(PlayerUnit thisUnit) => true;
    
    public bool requiresConfirm; // fillable via ScriptableObject interface
    public bool revertable; // fillable via ScriptableObject interface

    // i don't like this but I'm also on a plane. Make this an interface IRevertable or something
    public virtual void Revert(PlayerUnit thisUnit){}
}