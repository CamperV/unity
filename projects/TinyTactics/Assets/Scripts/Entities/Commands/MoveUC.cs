using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Extensions;

public class MoveUC : UnitCommand
{
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
 

    // case State.Moving:
    //     if (cancelSignal) {
    //         cancelSignal = false;

    //         StopAllCoroutines();
    //         spriteAnimator.ClearStacks();

    //         unitMap.ClearReservation(_reservedGridPosition);
    //         UndoMovement();

    //     } else {
    //         unitMap.MoveUnit(this, _reservedGridPosition);
    //     }
    //     break;

    public override void ContextualInteractAt(PlayerUnit thisUnit, GridPosition interactAt, bool auxiliaryInteract) {
        if (interactAt == thisUnit.gridPosition) {
            Deactivate(thisUnit); // ie Cancel
        }

        // // pathToMouseOver is updated right before this in ContextualNoUpdate
        // // if a path exists to the destination, smoothly move along the path
        // // after reaching your destination, officially move via unitMap
        // if (pathToMouseOver != null) {
        //     StartCoroutine(
        //         spriteAnimator.SmoothMovementPath<GridPosition>(pathToMouseOver, battleMap)
        //     );

        //     unitMap.ReservePosition(this, gp);
        //     _reservedGridPosition = gp;  // save for ContextualNoInteract to move via unitMap
        //     moveAvailable = false;

        //     ChangeState(State.Moving);
        //     auxiliaryInteractFlag = auxiliaryInteract;

        //     FireOnMoveEvent(pathToMouseOver);

        //     //
        //     personalAudioFX.PlayInteractFX();
        // }
    }

    public override void ContextualNoInteract(PlayerUnit thisUnit) {
        // // this is where we constantly recalculate/show the path to your mouse destination

        // // when the mouse-on-grid changes:
        // if (battleMap.CurrentMouseGridPosition != _previousMouseOver) {
        //     battleMap.ClearDisplayPath();

        //     if (battleMap.MouseInBounds) {
        //         pathToMouseOver = moveRange.BFS(gridPosition, battleMap.CurrentMouseGridPosition);
        //         _previousMouseOver = battleMap.CurrentMouseGridPosition;

        //         if (pathToMouseOver != null) battleMap.DisplayPath(pathToMouseOver);
        //     }
        // }
        // break;

        // ///////////////////////////////////////////////////////////
        // // Every frame that we are moving (after MoveSelection), //
        // // check the spriteAnimator. As soon as we stop moving,  //
        // // update our position via unitMap and ChangeState       //
        // ///////////////////////////////////////////////////////////
        // case State.Moving:
        //     if (cancelSignal) {
        //         ChangeState(State.Idle);
        //         break;
        //     }

        //     if (spriteAnimator.isMoving) {    
        //         // just spin

        //     // we've finished moving
        //     } else {

        //         // if this interact was fired via Middle-Mouse, immediately wait
        //         if (auxiliaryInteractFlag) {
        //             auxiliaryInteractFlag = false;
        //             Wait();

        //         // if there's an in-range enemy, go to AttackSelection
        //         // if (attackAvailable && ValidAttackExistsFrom(_reservedGridPosition)) {
        //         } else if (attackAvailable) {
        //             ChangeState(State.AttackSelection);
                    
        //         } else {
        //             FinishTurn();
        //             ChangeState(State.Idle);
        //         }
        //     }
        //     break;
    }
}
