using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Extensions;

[CreateAssetMenu (menuName = "UnitCommands/ChargeUC")]
public class ChargeUC : MoveUC
{
    public int damageBonus;

    public override ExitSignal ActiveInteractAt(PlayerUnit thisUnit, GridPosition interactAt, bool auxiliaryInteract) {
        if (interactAt == thisUnit.gridPosition)
            return ExitSignal.NoStateChange;
        
        // pathToMouseOver is updated right before this in ContextualNoUpdate
        // if a path exists to the destination, smoothly move along the path
        // after reaching your destination, officially move via unitMap
        if (MoveUC.pathToMouseOver != null) {
            thisUnit.personalAudioFX.PlayInteractFX();

            Utils.DelegateCoroutineTo(thisUnit,
                thisUnit.spriteAnimator.SmoothMovementPath<GridPosition>(MoveUC.pathToMouseOver, thisUnit.battleMap)
            );
            thisUnit.ReservePosition(interactAt);
            thisUnit.FireOnMoveEvent(MoveUC.pathToMouseOver);

            // from ChargeMut
            for (int i = 0; i < MoveUC.pathToMouseOver.Count-1; i++) {
                thisUnit.statusManager.AddValuedStatus<OneTimeDamageBuff>(name, damageBonus);
            }
            thisUnit.statusManager.movementBuffProviders.Add(name);
            // from ChargeMut

            // Complete -> "Change to InProgress state after returning"
            return ExitSignal.NextState;
        }

        // Incomplete -> "Do not change to InProgress state after returning"
        return ExitSignal.NoStateChange;
    }

    public override ExitSignal FinishCommand(PlayerUnit thisUnit, bool auxiliaryInteract) {
        thisUnit.ClaimReservation();
        return ExitSignal.ContinueTurn;
    }
}
