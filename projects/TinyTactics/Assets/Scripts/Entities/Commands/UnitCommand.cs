using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Extensions;

public abstract class UnitCommand : ScriptableObject
{
    // model things like MoveSelection (previously in PlayerUnitFSM) here
    //
    // Activate
    // Command
    // ContextualInteracts
    // ContextualNoInteract (no mouse click)

    public abstract void Activate(PlayerUnit thisUnit);
    public abstract void Deactivate(PlayerUnit thisUnit);
    public abstract void ContextualInteractAt(PlayerUnit thisUnit, GridPosition interactAt, bool auxiliaryInteract);
    public abstract void ContextualNoInteract(PlayerUnit thisUnit);
}