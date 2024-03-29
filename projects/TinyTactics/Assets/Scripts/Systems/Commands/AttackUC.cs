using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Extensions;
using System.Linq;

[CreateAssetMenu (menuName = "UnitCommands/AttackUC")]
public class AttackUC : TargetableUC
{
    // waiting until an Engagement is done animating and resolving casualties
    public static bool _engagementResolveFlag = false;

    public override void Activate(PlayerUnit thisUnit) {        
        thisUnit.UpdateThreatRange(
            standing: true,
            minRange: thisUnit.EquippedWeapon.MIN_RANGE,
            maxRange: thisUnit.EquippedWeapon.MAX_RANGE
        );
        Utils.DelegateLateFrameTo(thisUnit, () => DisplayTargetRange(thisUnit));

        // reset this to make sure the same-frame mouseOver works
        TargetableUC._previousMouseOver = null;
    }

    public override void Deactivate(PlayerUnit thisUnit) {
        UnitSelectionSystem.inst.Unlock();

        thisUnit.battleMap.ResetHighlightTiles();
        thisUnit.battleMap.ResetHighlight();
        //
        EngagementPreviewSystem.inst.DisablePreview(null);
    }

    protected override bool ValidTarget(PlayerUnit thisUnit, GridPosition interactAt) {
        return thisUnit.attackRange.ValidTarget(interactAt) && EnemyAt(thisUnit, interactAt) != null;
    }

    protected override void Execute(PlayerUnit thisUnit, GridPosition interactAt) {
        EngagementPreviewSystem.inst.DisablePreview(null);

        EnemyUnit enemy = EnemyAt(thisUnit, interactAt);

        _engagementResolveFlag = true;
        Engagement engagement = EngagementSystem.CreateEngagement(thisUnit, enemy);
        EngagementSystem.inst.Resolve(engagement);

        // wait until the engagement has ended
        // once the engagement has processed, resolve the casualties
        // once the casualties are resolved, EndTurnSelectedUnit()
        EngagementSystem.inst.ExecuteAfterResolving(() => _engagementResolveFlag = false);
    }

    protected override void ResetValidMouseOver(PlayerUnit thisUnit) {
        base.ResetValidMouseOver(thisUnit);
        
        EngagementPreviewSystem.inst.DisablePreview(null);
    }

    protected override void ValidMouseOver(PlayerUnit thisUnit, GridPosition hoverOver) {        
        base.ValidMouseOver(thisUnit, hoverOver);

        // create and display EngagementPreviews here
        EnemyUnit enemy = EnemyAt(thisUnit, hoverOver);
        EngagementPreviewSystem.inst.EnablePreview(EngagementSystem.CreateEngagement(thisUnit, enemy));
    }

    ////////////////////////////////////////////////////////////////////
    // Every frame that we're animating our attack (after selecting), //
    // check the spriterAnimator. As soon as we stop animating,       //
    // disable your phase and move into Idle                          //
    ////////////////////////////////////////////////////////////////////
    public override ExitSignal InProgressUpdate(PlayerUnit thisUnit) {
        bool waitCondition = _engagementResolveFlag || thisUnit.spriteAnimator.isMoving || thisUnit.spriteAnimator.isAnimating;
        return (waitCondition) ? ExitSignal.NoStateChange : ExitSignal.NextState;
    }

    // always end turn after attacking
    public override ExitSignal FinishCommand(PlayerUnit thisUnit, bool auxiliaryInteract) {
        return ExitSignal.ForceFinishTurn;
    }

    // additionally, this command is only available if there's a ValidTarget to be made
    public override bool IsAvailableAux(PlayerUnit thisUnit) {
        return ValidTargetExistsFrom(thisUnit, thisUnit.gridPosition);
    }

    //
    //
    private EnemyUnit EnemyAt(PlayerUnit thisUnit, GridPosition gp) {
        if (!thisUnit.battleMap.IsInBounds(gp)) return null;

        Unit unit = thisUnit.unitMap.UnitAt(gp);
        if (unit != null && unit.GetType() == typeof(EnemyUnit)) {
            return unit as EnemyUnit;
        } else {
            return null;
        }
    }

    private bool ValidTargetExistsFrom(PlayerUnit thisUnit, GridPosition fromPosition) {        
        EnemyUnitController enemyUC = thisUnit.enemyUnitController;

        bool validTargetForAnyWeapon = false;
        foreach (Weapon w in thisUnit.inventory.Weapons) {
            TargetRange standing = TargetRange.Standing(fromPosition, w.MIN_RANGE, w.MAX_RANGE);
            validTargetForAnyWeapon |= enemyUC.GetActiveUnits().Where(enemy => standing.ValidTarget(enemy.gridPosition)).Any();
        }
        return validTargetForAnyWeapon;
    }
}
