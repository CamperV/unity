using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Extensions;
using System.Linq;

[CreateAssetMenu (menuName = "UnitCommands/HealUC")]
public class HealUC : TargetableUC
{
    public int healAmount; // set via SO interface

    // this will change every single instance of HealUC that exists, its an SO
    // but this will never exist more than once. Just do it, coward 
    private PlayerUnit _targetAlly;

    protected override bool ValidTarget(PlayerUnit thisUnit, GridPosition interactAt) {
        PlayerUnit ally = AllyAt(thisUnit, interactAt);
        return thisUnit.attackRange.ValidTarget(interactAt) && ally != null && ally.unitStats.MissingHP() > 0;
    }

    protected override void Execute(PlayerUnit thisUnit, GridPosition interactAt) {
        _targetAlly = AllyAt(thisUnit, interactAt);
        _targetAlly.HealAmount(healAmount);
    }

    protected override void ResetValidMouseOver(PlayerUnit thisUnit) {
        DisplayTargetRange(thisUnit);
        thisUnit.playerUnitController.Unlock();
    }

    protected override void ValidMouseOver(PlayerUnit thisUnit, GridPosition hoverOver) {
        thisUnit.battleMap.Highlight(thisUnit.battleMap.CurrentMouseGridPosition, tileVisuals.altColor);
        thisUnit.playerUnitController.Lock();
    }

    public override ExitSignal InProgressUpdate(PlayerUnit thisUnit) {
        bool waitCondition = _targetAlly.spriteAnimator.isMoving || _targetAlly.spriteAnimator.isAnimating;
        return (waitCondition) ? ExitSignal.NoStateChange : ExitSignal.NextState;
    }

    // always end turn after attacking
    public override ExitSignal FinishCommand(PlayerUnit thisUnit, bool auxiliaryInteract) {
        return ExitSignal.ForceFinishTurn;
    }

    //
    private PlayerUnit AllyAt(PlayerUnit thisUnit, GridPosition gp) {
        if (!thisUnit.battleMap.IsInBounds(gp)) return null;

        Unit unit = thisUnit.unitMap.UnitAt(gp);
        if (unit != null && unit.GetType() == typeof(PlayerUnit)) {
            return (unit as PlayerUnit);
        } else {
            return null;
        }
    }

    protected override void DisplayTargetRange(PlayerUnit thisUnit) {
        thisUnit.attackRange.Display(thisUnit.battleMap, tileVisuals.color);
        thisUnit.battleMap.Highlight(thisUnit.gridPosition, Palette.selectColorWhite);
    }
}
