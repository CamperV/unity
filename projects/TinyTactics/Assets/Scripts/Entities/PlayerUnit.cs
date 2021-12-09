using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUnit : Unit, IStateMachine<PlayerUnit.PlayerUnitFSM>
{
    public enum PlayerUnitFSM {
        Idle,
        MoveSelection,
        Moving,
        AttackSelection,
        Attacking
    }
    [SerializeField] public PlayerUnitFSM state { get; set; } = PlayerUnitFSM.Idle;

    public bool blocking = false;
    public bool cancelSignal = false;

    void Start() {
        // register any relevant events
        EventManager.inst.inputController.RightMouseClickEvent += _ => ChangeState(PlayerUnit.PlayerUnitFSM.Idle);
        moveRange = null;
        attackRange = null;
        EnterState(PlayerUnitFSM.Idle);
    }

    void Update() {
        ContextualNoInteract();
    }

    public void ChangeState(PlayerUnitFSM newState) {
        if (newState == state) return;
        
        ExitState(state);
        EnterState(newState);
    }

    public void EnterState(PlayerUnitFSM enteringState) {
        state = enteringState;

        // debug
        GetComponentInChildren<TextMeshPro>().SetText(state.ToString());

        switch (state) {
            // when you're entering Idle, it's from being selected
            // therefore, reset your controller's selections
            case PlayerUnitFSM.Idle:
                playerUnitController.ClearSelection();
                break;

            // re-calc move range, and display it
            case PlayerUnitFSM.MoveSelection:
                UpdateThreatRange();
                StartCoroutine( Utils.LateFrame(DisplayThreatRange) );
                break;

            case PlayerUnitFSM.Moving:
                blocking = true;
                break;

            case PlayerUnitFSM.AttackSelection:
                moveRange = GenerateMoveRange(gridPosition, 0);
                attackRange = GenerateAttackRange(unitStats.MIN_RANGE, unitStats.MAX_RANGE);

                StartCoroutine( Utils.LateFrame(DisplayThreatRange) );
                break;

            case PlayerUnitFSM.Attacking:
                break;
        }
    }

    public void ExitState(PlayerUnitFSM exitingState) {
        Debug.Log($"{this} exiting state {exitingState}");

        switch (exitingState) {
            case PlayerUnitFSM.Idle:
                break;

            case PlayerUnitFSM.MoveSelection:
                battleMap.ResetHighlight();
                break;

            case PlayerUnitFSM.Moving:
                unitMap.MoveUnit(this, _reservedGridPosition);
                blocking = false;
                break;

            case PlayerUnitFSM.AttackSelection:
                battleMap.ResetHighlight();  
                break;

            case PlayerUnitFSM.Attacking:
                break;
        }
        state = PlayerUnitFSM.Idle;
    }

    public void ContextualInteractAt(GridPosition gp) {
        switch (state) {

            ///////////////////////////////////////////////
            // ie Active the unit, go to select movement //
            ///////////////////////////////////////////////
            case PlayerUnitFSM.Idle:
                ChangeState(PlayerUnitFSM.MoveSelection);
                break;

            ////////////////////////////////////////////////////////////////////////
            // The GridPosition gp is the selection made to move the unit towards //
            ////////////////////////////////////////////////////////////////////////
            case PlayerUnitFSM.MoveSelection:
                if (gp == gridPosition) {
                    ChangeState(PlayerUnitFSM.AttackSelection);

                // else if it's a valid movement to be had:
                } else {
                    Path<GridPosition>? pathTo = moveRange.BFS(gridPosition, gp);

                    // if a path exists to the destination, smoothly move along the path
                    // after reaching your destination, officially move via unitMap
                    if (pathTo != null) {
                        StartCoroutine( spriteAnimator.SmoothMovementPath<GridPosition>(pathTo, battleMap) );

                        unitMap.ReservePosition(this, gp);
                        _reservedGridPosition = gp;  // save for ContextualNoInteract to move via unitMap

                        ChangeState(PlayerUnitFSM.Moving);
                    } else {
                        Debug.Log($"Found no path from {gridPosition} to {gp}");
                        ChangeState(PlayerUnitFSM.Idle);
                    }
                }
                break;

            case PlayerUnitFSM.Moving:
                break;

            ///////////////////////////////////////////////////////////////////////
            // The GridPosition gp is the selection made to have the unit attack //
            ///////////////////////////////////////////////////////////////////////
            case PlayerUnitFSM.AttackSelection:
                if (gp == gridPosition) {
                    ChangeState(PlayerUnitFSM.Idle);

                } else {
                    if (attackRange.ValidAttack(gp)) {
                        StartCoroutine( spriteAnimator.BumpTowards<GridPosition>(gp, battleMap) );

                        ChangeState(PlayerUnitFSM.Attacking);
                    } else {
                        Debug.Log($"No valid attack exists from {gridPosition} to {gp}");
                        ChangeState(PlayerUnitFSM.Idle);
                    }
                }
                break;

            case PlayerUnitFSM.Attacking:
                break;
        }
    }

    public void ContextualNoInteract() {
        switch (state) {
            case PlayerUnitFSM.Idle:
                break;

            case PlayerUnitFSM.MoveSelection:
                break;

            ///////////////////////////////////////////////////////////
            // Every frame that we are moving (after MoveSelection), //
            // check the spriteAnimator. As soon as we stop moving,  //
            // update our position via unitMap and ChangeState       //
            ///////////////////////////////////////////////////////////
            case PlayerUnitFSM.Moving:
                if (spriteAnimator.isMoving) {    
                    // just spin

                // we've finished moving
                } else {
                    if (cancelSignal) {
                        cancelSignal = false;
                        ChangeState(PlayerUnitFSM.Idle);
                    } else {
                        ChangeState(PlayerUnitFSM.AttackSelection);
                    }
                }
                break;

            case PlayerUnitFSM.AttackSelection:
                break;

            ////////////////////////////////////////////////////////////////////
            // Every frame that we're animating our attack (after selecting), //
            // check the spriterAnimator. As soon as we stop animating,       //
            // disable your phase and move into Idle                          //
            ////////////////////////////////////////////////////////////////////
            case PlayerUnitFSM.Attacking:
                if (spriteAnimator.isMoving || spriteAnimator.isAnimating) {    
                    // just spin
                } else {
                    if (cancelSignal) {
                        cancelSignal = false;
                        ChangeState(PlayerUnitFSM.Idle);
                    } else {
                        ChangeState(PlayerUnitFSM.Idle);
                    }
                }
                break;
        }
    }

    // this essentially is an "undo" for us
    // undo all the way to Idle
    public void Cancel() {
        if (state == PlayerUnitFSM.Idle) return;

        switch (state) {
            case PlayerUnitFSM.Moving:
            case PlayerUnitFSM.Attacking:
                cancelSignal = true;
                break;

            case PlayerUnitFSM.MoveSelection:
            case PlayerUnitFSM.AttackSelection:
                ChangeState(PlayerUnitFSM.Idle);
                break;
        }
    }

    // this needs to run at the end of the frame
    // this is because of our decoupled event processing
    // basically, the PlayerUnits are displaying  before the enemy units drop the display
    //
    // always display AttackRange first, because it is partially overwritten by MoveRange by definition
    protected void DisplayThreatRange() {
        attackRange.Display(battleMap);
        moveRange.Display(battleMap);

    	foreach (GridPosition gp in ThreatenedRange()) {
			if (moveRange.field.ContainsKey(gp)) {
				battleMap.Highlight(gp, Constants.threatColorIndigo);
			}
		}

        battleMap.Highlight(gridPosition, Constants.selectColorWhite);
    }

    private IEnumerable<GridPosition> ThreatenedRange() {
		HashSet<GridPosition> threatened = new HashSet<GridPosition>();

		foreach (EnemyUnit unit in enemyUnitController.entities) {
            if (unit.attackRange == null) {
                Debug.Log($"Triggered update of {unit}'s attackRange");
                unit.UpdateThreatRange();
                Debug.Log($"{unit} has an attack range that == null? {unit.attackRange == null}");
            }
			threatened.UnionWith(unit.attackRange.field.Keys);
		}

		foreach (GridPosition gp in threatened) yield return gp;
	}
}
