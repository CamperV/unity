using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Extensions;
using UnityEngine.UI;

public abstract class UnitCommand : ScriptableObject
{
    public enum ExitSignal {
        NoStateChange,
        NextState,
        ContinueTurn,
        ForceFinishTurn
    }
    public string name; // fillable via ScriptableObject interface
    public Sprite sprite; // fillable via ScriptableObject interface

    public abstract void Activate(PlayerUnit thisUnit); /* Initial activation */
    public abstract void Deactivate(PlayerUnit thisUnit); /* De-activation, ie Cancel */
    public abstract ExitSignal ActiveInteractAt(PlayerUnit thisUnit, GridPosition interactAt, bool auxiliaryInteract); /* Interaction/execution while active */
    public abstract void ActiveUpdate(PlayerUnit thisUnit); /* Always-on while active */
    public abstract ExitSignal InProgressUpdate(PlayerUnit thisUnit); /* Always-on after executing */
    public abstract ExitSignal FinishCommand(PlayerUnit thisUnit, bool auxiliaryInteract); /* Finish, usually responsible for executing state changes like Unit Movement */
}