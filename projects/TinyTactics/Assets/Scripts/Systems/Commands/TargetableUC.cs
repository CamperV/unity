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
    public static GridPosition? _previousMouseOver; // for MoveSelection and AttackSelection (ContextualNoInteract)

    [SerializeField] protected TileVisuals tileVisuals;

    public override void Activate(PlayerUnit thisUnit) {     
        thisUnit.UpdateThreatRange(standing: true, minRange: minRange, maxRange: maxRange);
        Utils.DelegateLateFrameTo(thisUnit, () => DisplayTargetRange(thisUnit));

        _previousMouseOver = null;
    }

    public override void Deactivate(PlayerUnit thisUnit) {
        thisUnit.playerUnitController.Unlock();

        thisUnit.battleMap.ResetHighlightTiles();
        thisUnit.battleMap.ResetHighlight();
    }

    public override ExitSignal ActiveInteractAt(PlayerUnit thisUnit, GridPosition interactAt, bool auxiliaryInteract) {
        if (interactAt == thisUnit.gridPosition)
            return ExitSignal.NoStateChange;

        // if there's a ValidTarget on the mouseclick'd area
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
            thisUnit.playerUnitController.Unlock();
            
            if (ValidTarget(thisUnit, thisUnit.battleMap.CurrentMouseGridPosition)) {
                thisUnit.playerUnitController.Lock();
                //
                ValidMouseOver(thisUnit, thisUnit.battleMap.CurrentMouseGridPosition);
            }
        } else {
            // Debug.Log($"Active, previous: {_previousMouseOver}");
        }
    }

    protected virtual void ResetValidMouseOver(PlayerUnit thisUnit) {
        DisplayTargetRange(thisUnit);

        // and also other virtual stuff
    }
    protected virtual void ValidMouseOver(PlayerUnit thisUnit, GridPosition hoverOver) {
        // highlight the ground beneath
        thisUnit.battleMap.Highlight(hoverOver, tileVisuals.altColor);

        // and also do other virtual stuff
    }

    protected virtual void DisplayTargetRange(PlayerUnit thisUnit) {
        thisUnit.attackRange.Display(thisUnit.battleMap, tileVisuals.color, tileVisuals.tile);
        thisUnit.battleMap.Highlight(thisUnit.gridPosition, Palette.selectColorWhite);
    }
}
