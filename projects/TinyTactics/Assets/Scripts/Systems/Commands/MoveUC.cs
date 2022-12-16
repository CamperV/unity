using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using TMPro;
using Extensions;

[CreateAssetMenu (menuName = "UnitCommands/MoveUC")]
public class MoveUC : UnitCommand
{
    // we can actually keep some state here: there should never be two MoveUC's Activated at the same time
    public static GridPosition? _previousMouseOver = null; // for MoveSelection and AttackSelection (ContextualNoInteract)
    
    // I really hate that I'm keeping so much data here
    // but only one MoveUC will ever be active, so we can store it in the class
    // maybe I should encapsulate it, and then write the behavior elsewhere
    // but first... write it!
    public static Path<GridPosition> _mouseOverPath;
    public static MoveRange _activeMoveRange;
    public static List<GridPosition> _waypoints;
    public static List<Path<GridPosition>> _pathSegments;

    [SerializeField] protected TileVisuals tileVisuals;

    public override void Activate(PlayerUnit thisUnit) {
        thisUnit.UpdateThreatRange();
        _activeMoveRange = thisUnit.moveRange;

        // re-calc move range, and display it
        Utils.DelegateLateFrameTo(thisUnit, () => DisplayMoveRange(thisUnit, thisUnit.moveRange));
        UIManager.inst.EnableUnitDetail(thisUnit);

        _waypoints = new List<GridPosition>{thisUnit.gridPosition};
        _pathSegments = new List<Path<GridPosition>>();
    }

    public override void Deactivate(PlayerUnit thisUnit) {
        thisUnit.battleMap.ResetHighlightTiles();
        thisUnit.battleMap.ResetHighlight();
        thisUnit.battleMap.ClearDisplayPath();
        //
        UIManager.inst.DisableUnitDetail();

        _waypoints = null;
        _activeMoveRange = null;
        _pathSegments = null;
    }

    public override ExitSignal ActiveInteractAt(PlayerUnit thisUnit, GridPosition interactAt, bool auxiliaryInteract) {
        if (interactAt == thisUnit.gridPosition)
            return ExitSignal.NoStateChange;
        
        // use this to create waypoints, and don't process anything
        if (auxiliaryInteract) {
            _AddWaypoint(thisUnit, interactAt);
            return ExitSignal.NoStateChange;
        }

        // _mouseOverPath is updated right before this in ContextualNoUpdate
        // if a path exists to the destination, smoothly move along the path
        // after reaching your destination, officially move via unitMap
        if (_mouseOverPath != null) {
            thisUnit.personalAudioFX.PlayInteractFX();

            Utils.DelegateCoroutineTo(thisUnit,
                thisUnit.spriteAnimator.SmoothMovementPath<GridPosition>(_mouseOverPath, thisUnit.battleMap)
            );
            thisUnit.ReservePosition(interactAt);
            thisUnit.FireOnMoveEvent(_mouseOverPath);

            // for other inheriting MoveUCs, like Charge or Scurry
            ExecuteAdditionalOnMove(thisUnit, _mouseOverPath);

            // Complete -> "Change to InProgress state after returning"
            return ExitSignal.NextState;
        }

        // Incomplete -> "Do not change to InProgress state after returning"
        return ExitSignal.NoStateChange;
    }

    // this is where we constantly recalculate/show the path to your mouse destination
    // this will fire on Update(), but should only fire when CommandActive is the state
    public override void ActiveUpdate(PlayerUnit thisUnit) {
        if (!thisUnit.battleMap.MouseInBounds) return;

        if (thisUnit.battleMap.CurrentMouseGridPosition != _previousMouseOver) {    // when the mouse-on-grid changes:
            thisUnit.battleMap.ClearDisplayPath();

            // user can input waypoints, but I need to generate a new FlowField/MoveRange for it
            // keep the entire system here. Honestly since we're already storing other crap statically...

            // starting from:
            GridPosition finalWaypoint = (_waypoints.Count > 0) ? _waypoints.Last() : thisUnit.gridPosition;
            Path<GridPosition> finalSegment = _activeMoveRange.BFS(finalWaypoint, thisUnit.battleMap.CurrentMouseGridPosition);               

            if (finalSegment != null) {
                var segmentsCopy = new List<Path<GridPosition>>(_pathSegments);
                segmentsCopy.Add(finalSegment);

                _mouseOverPath = Path<GridPosition>.MergePaths(segmentsCopy);
                thisUnit.battleMap.DisplayPath(_mouseOverPath, _waypoints);

            // still display the other segments
            } else if (_pathSegments.Count > 0) {
                thisUnit.battleMap.DisplayPath(
                    Path<GridPosition>.MergePaths( new List<Path<GridPosition>>(_pathSegments) ),
                    _waypoints
                );
            }

            // finally,
            _previousMouseOver = thisUnit.battleMap.CurrentMouseGridPosition;
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
        // return (auxiliaryInteract) ? ExitSignal.ForceFinishTurn : ExitSignal.ContinueTurn;
        return ExitSignal.ContinueTurn;
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

        _waypoints = null;
    }

    protected virtual void ExecuteAdditionalOnMove(PlayerUnit thisUnit, Path<GridPosition> pathTaken) {}

    protected virtual void DisplayMoveRange(PlayerUnit thisUnit, MoveRange moveRange) {   
        moveRange.Display(thisUnit.battleMap, tileVisuals.color, tileVisuals.tile);

    	foreach (GridPosition gp in ThreatenedRange(thisUnit)) {
			if (moveRange.field.ContainsKey(gp)) {
				thisUnit.battleMap.Highlight(gp, tileVisuals.altColor);
			}
		}

        thisUnit.battleMap.Highlight(thisUnit.gridPosition, Palette.selectColorWhite);
    }

    protected virtual void DisplayMoveRange(PlayerUnit thisUnit, MoveRange moveRange, float alphaOverride) {   
        moveRange.Display(thisUnit.battleMap, tileVisuals.color.WithAlpha(alphaOverride), tileVisuals.tile);

    	foreach (GridPosition gp in ThreatenedRange(thisUnit)) {
			if (moveRange.field.ContainsKey(gp)) {
				thisUnit.battleMap.Highlight(gp, tileVisuals.altColor.WithAlpha(alphaOverride));
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

    // nasty static storage for now
    private void _AddWaypoint(PlayerUnit thisUnit, GridPosition gp) {
        _previousMouseOver = null; // force a state update for displaying purposes/recalculating the paths


        Path<GridPosition> pathSegment = _activeMoveRange.BFS(_waypoints.Last(), gp);
        
        if (pathSegment != null) {
            _pathSegments.Add(pathSegment);
            _waypoints.Add(gp);
            
            int remainingMovement = thisUnit.statSystem.MOVE - _pathSegments.Sum(pathSeg => pathSeg.Count-1);
            _activeMoveRange = thisUnit.unitPathfinder.GenerateFlowField<MoveRange>(_waypoints.Last(), range: remainingMovement);
            _activeMoveRange.RegisterValidMoveToFunc(thisUnit.unitMap.CanMoveInto);

            // re-display move range
            thisUnit.battleMap.ResetHighlightTiles();
            thisUnit.battleMap.ResetHighlight();
            thisUnit.battleMap.ClearDisplayPath();

            Utils.DelegateLateFrameTo(thisUnit, () => DisplayMoveRange(thisUnit, thisUnit.moveRange, 0.25f));
            Utils.DelegateLateFrameTo(thisUnit, () => DisplayMoveRange(thisUnit, _activeMoveRange));
        }
    }
}
