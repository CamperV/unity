using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Extensions;

// For cancelling Movement that is ongoing:
// if (cancelSignal) {
//     cancelSignal = false;

//     StopAllCoroutines();
//     spriteAnimator.ClearStacks();

//     unitMap.ClearReservation(_reservedGridPosition);
//     UndoMovement();

// } else {
//     unitMap.MoveUnit(this, _reservedGridPosition);
// }

// if (cancelSignal) {
//     ChangeState(PlayerUnitFSM.Idle);
//     break;
// }

// if (spriteAnimator.isMoving) {    
//     // just spin

// // we've finished moving
// } else {

//     // if this interact was fired via Middle-Mouse, immediately wait
//     if (auxiliaryInteractFlag) {
//         auxiliaryInteractFlag = false;
//         Wait();

//     // if there's an in-range enemy, go to AttackSelection
//     // if (attackAvailable && ValidAttackExistsFrom(_reservedGridPosition)) {
//     } else if (attackAvailable) {
//         ChangeState(PlayerUnitFSM.AttackSelection);
        
//     } else {
//         FinishTurn();
//         ChangeState(PlayerUnitFSM.Idle);
//     }
// }

// private bool ValidAttackExistsFrom(GridPosition fromPosition) {
//     AttackRange standing = AttackRange.Standing(fromPosition, equippedWeapon.weaponStats.MIN_RANGE, equippedWeapon.weaponStats.MAX_RANGE);
//     return enemyUnitController.activeUnits.Where(enemy => standing.ValidAttack(enemy.gridPosition)).Any();
// }

[CreateAssetMenu (menuName = "UnitCommands/MoveUC")]
public class MoveUC : UnitCommand
{
    // we can actually keep some state here: there should never be two MoveUC's Activated at the same time
    public static GridPosition _previousMouseOver; // for MoveSelection and AttackSelection (ContextualNoInteract)
    public static Path<GridPosition>? pathToMouseOver;

    public override void Activate(PlayerUnit thisUnit) {
        // re-calc move range, and display it
        thisUnit.UpdateThreatRange();
        Utils.DelegateLateFrameTo(thisUnit, thisUnit.DisplayThreatRange);
        UIManager.inst.EnableUnitDetail(thisUnit);
    }

    public override void Deactivate(PlayerUnit thisUnit) {
        thisUnit.battleMap.ResetHighlight();
        thisUnit.battleMap.ClearDisplayPath();
        //
        UIManager.inst.DisableUnitDetail();
    }

    public override ExitSignal ActiveInteractAt(PlayerUnit thisUnit, GridPosition interactAt, bool auxiliaryInteract) {
        if (interactAt == thisUnit.gridPosition)
            return ExitSignal.NoStateChange;
        
        // pathToMouseOver is updated right before this in ContextualNoUpdate
        // if a path exists to the destination, smoothly move along the path
        // after reaching your destination, officially move via unitMap
        if (pathToMouseOver != null) {
            Utils.DelegateCoroutineTo(thisUnit,
                thisUnit.spriteAnimator.SmoothMovementPath<GridPosition>(pathToMouseOver, thisUnit.battleMap)
            );
            thisUnit.ReservePosition(interactAt);

            thisUnit.FireOnMoveEvent(pathToMouseOver);
            thisUnit.personalAudioFX.PlayInteractFX();

            // Complete -> "Change to InProgress state after returning"
            return ExitSignal.NextState;
        }

        // Incomplete -> "Do not change to InProgress state after returning"
        return ExitSignal.NoStateChange;
    }

    // this is where we constantly recalculate/show the path to your mouse destination
    // this will fire on Update(), but should only fire when CommandActive is the state
    public override void ActiveUpdate(PlayerUnit thisUnit) {
        if (thisUnit.battleMap.CurrentMouseGridPosition != _previousMouseOver) {    // when the mouse-on-grid changes:
            thisUnit.battleMap.ClearDisplayPath();

            if (thisUnit.battleMap.MouseInBounds) {
                pathToMouseOver = thisUnit.moveRange.BFS(thisUnit.gridPosition, thisUnit.battleMap.CurrentMouseGridPosition);
                _previousMouseOver = thisUnit.battleMap.CurrentMouseGridPosition;

                if (pathToMouseOver != null) thisUnit.battleMap.DisplayPath(pathToMouseOver);
            }
        }
    }

    ///////////////////////////////////////////////////////////
    // Every frame that we are moving (after MoveSelection), //
    // check the spriteAnimator. As soon as we stop moving,  //
    // update our position via unitMap and ChangeState       //
    ///////////////////////////////////////////////////////////
    public override ExitSignal InProgressUpdate(PlayerUnit thisUnit) {
        return (thisUnit.spriteAnimator.isMoving) ? ExitSignal.NoStateChange : ExitSignal.NextState;
    }

    // if Auxiliary Interact was used, force a FinishTurn after moving (auto wait)
    public override ExitSignal FinishCommand(PlayerUnit thisUnit, bool auxiliaryInteract) {
        thisUnit.ClaimReservation();
        return (auxiliaryInteract) ? ExitSignal.ForceFinishTurn : ExitSignal.ContinueTurn;
    }
}
