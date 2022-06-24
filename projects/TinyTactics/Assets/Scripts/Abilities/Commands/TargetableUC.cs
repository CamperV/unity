using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Extensions;
using System.Linq;

public abstract class TargetableUC : UnitCommand
{
    public int minRange; // assign in ScriptableObject interface
    public int maxRange; // assign in ScriptableObject interface

    // we can actually keep some state here: there should never be two AttackUC's Activated at the same time
    public static GridPosition _previousMouseOver; // for MoveSelection and AttackSelection (ContextualNoInteract)

    public override void Activate(PlayerUnit thisUnit) {        
        thisUnit.UpdateThreatRange(standing: true, minRange: minRange, maxRange: maxRange);
        Utils.DelegateLateFrameTo(thisUnit, thisUnit.DisplayThreatRange);
    }

    public override void Deactivate(PlayerUnit thisUnit) {
        thisUnit.battleMap.ResetHighlight();
        //
        UIManager.inst.DisableUnitDetail();
    }

    public override ExitSignal ActiveInteractAt(PlayerUnit thisUnit, GridPosition interactAt, bool auxiliaryInteract) {
        if (interactAt == thisUnit.gridPosition)
            return ExitSignal.NoStateChange;

        // if there's a ValidAttack on the mouseclick'd area
        if (ValidTarget(thisUnit, interactAt)) {
            Execute(thisUnit, interactAt);
            return ExitSignal.NextState;
        }

        // Incomplete -> "Do not change to InProgress state after returning"
        return ExitSignal.NoStateChange;
    }

    protected abstract bool ValidTarget(PlayerUnit thisUnit, GridPosition interactAt);
    protected abstract void Execute(PlayerUnit thisUnit, GridPosition interactAt); 

    // this will fire on Update(), but should only fire when CommandActive is the state
    public override void ActiveUpdate(PlayerUnit thisUnit) {
        if (thisUnit.battleMap.CurrentMouseGridPosition != _previousMouseOver) {
            _previousMouseOver = thisUnit.battleMap.CurrentMouseGridPosition;

            ResetValidMouseOver(thisUnit);

            if (ValidTarget(thisUnit, thisUnit.battleMap.CurrentMouseGridPosition)) {
                ValidMouseOver(thisUnit, thisUnit.battleMap.CurrentMouseGridPosition);
            }
        }
    }

    protected abstract void ResetValidMouseOver(PlayerUnit thisUnit);
    protected abstract void ValidMouseOver(PlayerUnit thisUnit, GridPosition hoverOver);
}
