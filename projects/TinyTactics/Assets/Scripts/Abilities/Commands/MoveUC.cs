using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Extensions;

[CreateAssetMenu (menuName = "UnitCommands/MoveUC")]
public class MoveUC : UnitCommand
{
    // we can actually keep some state here: there should never be two MoveUC's Activated at the same time
    public static GridPosition _previousMouseOver; // for MoveSelection and AttackSelection (ContextualNoInteract)
    public static Path<GridPosition> pathToMouseOver;

    [SerializeField] protected TileVisuals tileVisuals;

    public override void Activate(PlayerUnit thisUnit) {
        // re-calc move range, and display it
        Utils.DelegateLateFrameTo(thisUnit,  () => DisplayMoveRange(thisUnit));
        UIManager.inst.EnableUnitDetail(thisUnit);
    }

    public override void Deactivate(PlayerUnit thisUnit) {
        thisUnit.battleMap.ResetHighlightTiles();
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
            thisUnit.personalAudioFX.PlayInteractFX();

            Utils.DelegateCoroutineTo(thisUnit,
                thisUnit.spriteAnimator.SmoothMovementPath<GridPosition>(pathToMouseOver, thisUnit.battleMap)
            );
            thisUnit.ReservePosition(interactAt);
            thisUnit.FireOnMoveEvent(pathToMouseOver);

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

    // this is only possible for a few UC
    // Attacking, or anything else that ends your turn, obviously cannot
    // but Movement might simply be used to preview an engagement
    //
    // NOTE: This is janky as hell. Really, I should be using Reservations in the UnitMap, but this kinda works...
    // there theoretically exists a period of time in which things snap around, as MoveUnit can move a Transform, like SpriteAnimator
    // however, the SmoothMovementGrid should override that. I don't know the order of operations vis-a-vis coroutines etc
    //
    // modifies gridPosition & updates threat range
    public override void Revert(PlayerUnit thisUnit) {
        thisUnit.ForfeitReservation();
        thisUnit.statusSystem.RevertMovementStatuses();
        thisUnit.RefreshInfo();
    }

    protected virtual void DisplayMoveRange(PlayerUnit thisUnit) {   
        if (thisUnit.moveRange == null) thisUnit.UpdateThreatRange();
        thisUnit.moveRange.Display(thisUnit.battleMap, tileVisuals.color, tileVisuals.tile);

    	foreach (GridPosition gp in ThreatenedRange(thisUnit)) {
			if (thisUnit.moveRange.field.ContainsKey(gp)) {
				thisUnit.battleMap.Highlight(gp, tileVisuals.altColor);
			}
		}

        thisUnit.battleMap.Highlight(thisUnit.gridPosition, Palette.selectColorWhite);
    }

    private IEnumerable<GridPosition> ThreatenedRange(PlayerUnit thisUnit) {
		HashSet<GridPosition> threatened = new HashSet<GridPosition>();

		foreach (EnemyUnit enemy in thisUnit.enemyUnitController.activeUnits) {
            if (enemy.attackRange == null) enemy.UpdateThreatRange();
			threatened.UnionWith(enemy.attackRange.field.Keys);
		}

		foreach (GridPosition gp in threatened) yield return gp;
	}
}
