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
        None,
        Movement,
        Attack,
        Wait
    }
    public CommandCategory commandCategory; // fillable via ScriptableObject interface

    public enum PanelCategory {
        Main,       // main commands added
        Default,    // default commands like Move and Attack
        Special     // default, but special commands, like Wait
    }
    public PanelCategory panelCategory; // fillable via ScriptableObject interface

    // default -1, slot in normally
    // otherwise, use this particular slot number when constructing UnitCommandPanel
    // ie, Move = 1, Attack = 2, Wait = 0
    public int panelSlot = -1;

    // this is optional. AttackUC has a custom one, so the weapon switcher can activate
    public UnitCommandVisual unitCommandVisualPrefab;
    
    public bool requiresConfirm; // fillable via ScriptableObject interface
    public bool revertable; // fillable via ScriptableObject interface

    // i don't like this but I'm also on a plane. Make this an interface IRevertable or something
    public virtual void Revert(PlayerUnit thisUnit){}

    public enum LimitType {
        Unlimited,
        Cooldown,
        LimitedUse
    }
    public LimitType limitType;

    public int cooldown; // default 0, which means no cooldowns
    public int remainingUses; // only necessary if LimitedUse is selected 

    // some commands can replace the default in the category; i.e ChargeUC replacing MoveUC
    public CommandCategory replaceDefault;

    public abstract void Activate(PlayerUnit thisUnit); /* Initial activation */
    public abstract void Deactivate(PlayerUnit thisUnit); /* De-activation, ie Cancel */
    public abstract ExitSignal ActiveInteractAt(PlayerUnit thisUnit, GridPosition interactAt, bool auxiliaryInteract); /* Interaction/execution while active */
    public abstract void ActiveUpdate(PlayerUnit thisUnit); /* Always-on while active */
    public abstract ExitSignal InProgressUpdate(PlayerUnit thisUnit); /* Always-on after executing */
    public abstract ExitSignal FinishCommand(PlayerUnit thisUnit, bool auxiliaryInteract); /* Finish, usually responsible for executing state changes like Unit Movement */

    // this is an optional method for specifying other ways a command can't be available.
    // For example, DefaultAttack can't be available unless there are targetable enemies in range
    public virtual bool IsAvailableAux(PlayerUnit thisUnit) => true;
}