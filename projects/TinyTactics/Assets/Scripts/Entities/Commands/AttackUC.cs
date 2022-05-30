using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Extensions;

[CreateAssetMenu (menuName = "UnitCommands/AttackUC")]
public class AttackUC : UnitCommand
{
    // we can actually keep some state here: there should never be two AttackUC's Activated at the same time
    public static GridPosition _previousMouseOver; // for MoveSelection and AttackSelection (ContextualNoInteract)

    // waiting until an Engagement is done animating and resolving casualties
    public static bool _engagementResolveFlag = false;


    public override void Activate(PlayerUnit thisUnit) {
        thisUnit.UpdateThreatRange(standing: true);
        Utils.DelegateLateFrameTo(thisUnit, thisUnit.DisplayThreatRange);
    }

    public override void Deactivate(PlayerUnit thisUnit) {
        thisUnit.battleMap.ResetHighlight();
        UIManager.inst.DisableEngagementPreview();
    }

    public override ExitSignal ActiveInteractAt(PlayerUnit thisUnit, GridPosition interactAt, bool auxiliaryInteract) {
        if (interactAt == thisUnit.gridPosition)
            return ExitSignal.NoStateChange;

        // if there's a ValidAttack on the mouseclick'd area
        if (thisUnit.attackRange.ValidAttack(interactAt) && EnemyAt(thisUnit, interactAt) != null) {
            EnemyUnit? enemy = EnemyAt(thisUnit, interactAt);

            _engagementResolveFlag = true;
            Engagement engagement = new Engagement(thisUnit, enemy);

            Utils.DelegateCoroutineTo(thisUnit,
                engagement.Resolve()
            );

            // wait until the engagement has ended
            // once the engagement has processed, resolve the casualties
            // once the casualties are resolved, EndTurnSelectedUnit()
            Utils.DelegateCoroutineTo(thisUnit,
                engagement.ExecuteAfterResolving(() => {
                    _engagementResolveFlag = false;
                })
            );

            return ExitSignal.NextState;
        }

        // Incomplete -> "Do not change to InProgress state after returning"
        return ExitSignal.NoStateChange;
    }

    // this is where we show EngagementPreviews and the like
    // this will fire on Update(), but should only fire when CommandActive is the state
    public override void ActiveUpdate(PlayerUnit thisUnit) {
        if (thisUnit.battleMap.CurrentMouseGridPosition != _previousMouseOver) {
            _previousMouseOver = thisUnit.battleMap.CurrentMouseGridPosition;

            // reset these
            thisUnit.DisplayThreatRange();
            UIManager.inst.DisableEngagementPreview();

            // when the mouse is over an enemy:
            if (thisUnit.attackRange.ValidAttack(thisUnit.battleMap.CurrentMouseGridPosition) && EnemyAt(thisUnit, thisUnit.battleMap.CurrentMouseGridPosition) != null) {
                EnemyUnit? enemy = EnemyAt(thisUnit, thisUnit.battleMap.CurrentMouseGridPosition);
                thisUnit.battleMap.Highlight(thisUnit.battleMap.CurrentMouseGridPosition, Palette.threatColorYellow);

                // create and display EngagementPreviews here
                UIManager.inst.EnableEngagementPreview( new Engagement(thisUnit, enemy), enemy.transform );
            }
        }
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

    //
    //
    private EnemyUnit? EnemyAt(PlayerUnit thisUnit, GridPosition gp) {
        if (!thisUnit.battleMap.IsInBounds(gp)) return null;

        Unit? unit = thisUnit.unitMap.UnitAt(gp);
        if (unit != null && unit.GetType() == typeof(EnemyUnit)) {
            return unit as EnemyUnit;
        } else {
            return null;
        }
    }
}
