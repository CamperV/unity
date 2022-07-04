using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Extensions;
using System.Linq;

[CreateAssetMenu (menuName = "UnitCommands/_deprecated_AttackUC")]
public class _deprecated_AttackUC : UnitCommand
{
    // we can actually keep some state here: there should never be two AttackUC's Activated at the same time
    public static GridPosition _previousMouseOver; // for MoveSelection and AttackSelection (ContextualNoInteract)

    // waiting until an Engagement is done animating and resolving casualties
    public static bool _engagementResolveFlag = false;
    
    [SerializeField] private TileVisuals tileVisuals;

    public override void Activate(PlayerUnit thisUnit) {        
        thisUnit.UpdateThreatRange(standing: true);
        Utils.DelegateLateFrameTo(thisUnit, () => DisplayAttackRange(thisUnit));
    }

    public override void Deactivate(PlayerUnit thisUnit) {
        thisUnit.playerUnitController.Unlock();

        thisUnit.battleMap.ResetHighlightTiles();
        thisUnit.battleMap.ResetHighlight();
        UIManager.inst.DisableEngagementPreview();
        //
        UIManager.inst.DisableUnitDetail();
    }

    public override ExitSignal ActiveInteractAt(PlayerUnit thisUnit, GridPosition interactAt, bool auxiliaryInteract) {
        if (interactAt == thisUnit.gridPosition)
            return ExitSignal.NoStateChange;

        // if there's a ValidTarget on the mouseclick'd area
        if (thisUnit.attackRange.ValidTarget(interactAt) && EnemyAt(thisUnit, interactAt) != null) {           
            EnemyUnit enemy = EnemyAt(thisUnit, interactAt);

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
            DisplayAttackRange(thisUnit);
            UIManager.inst.DisableEngagementPreview();
            //
            thisUnit.playerUnitController.Unlock();
            
            // when the mouse is over an enemy:
            if (thisUnit.attackRange.ValidTarget(thisUnit.battleMap.CurrentMouseGridPosition) && EnemyAt(thisUnit, thisUnit.battleMap.CurrentMouseGridPosition) != null) {
                thisUnit.playerUnitController.Lock();
                //
                EnemyUnit enemy = EnemyAt(thisUnit, thisUnit.battleMap.CurrentMouseGridPosition);
                thisUnit.battleMap.Highlight(thisUnit.battleMap.CurrentMouseGridPosition, tileVisuals.altColor);

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
            validTargetForAnyWeapon |= enemyUC.activeUnits.Where(enemy => standing.ValidTarget(enemy.gridPosition)).Any();
        }
        return validTargetForAnyWeapon;
    }

    private void DisplayAttackRange(PlayerUnit thisUnit) {
        thisUnit.attackRange.Display(thisUnit.battleMap, tileVisuals.color, tileVisuals.tile);
        thisUnit.battleMap.Highlight(thisUnit.gridPosition, Palette.selectColorWhite);
    }
}
