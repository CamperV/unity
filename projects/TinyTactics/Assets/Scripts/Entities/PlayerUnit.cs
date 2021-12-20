using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Extensions;

public class PlayerUnit : Unit, IStateMachine<PlayerUnit.PlayerUnitFSM>
{
    // IStateMachine<>
    public enum PlayerUnitFSM {
        Idle,
        MoveSelection,
        Moving,
        AttackSelection,
        Attacking,
        PreWait
    }
    [SerializeField] public PlayerUnitFSM state { get; set; } = PlayerUnitFSM.Idle;

    // cancels movement
    public bool cancelSignal = false;

    // waiting until an Engagement is done animating and resolving casualties
    private bool engagementResolveFlag = false;


    void Start() {
               
        // some init things that need to be taken care of
        unitStats.UpdateHP(unitStats.VITALITY, unitStats.VITALITY);

        originalColor = spriteRenderer.color;
        moveRange = null;
        attackRange = null;
        EnterState(PlayerUnitFSM.Idle);
    }

    void Update() { ContextualNoInteract(); }

    // IStateMachine<>
    public void ChangeState(PlayerUnitFSM newState) {
        if (newState == state) return;
        
        ExitState(state);
        EnterState(newState);
    }

    // IStateMachine<>
    public void InitialState() {
        ExitState(state);
        EnterState(PlayerUnitFSM.Idle);
    }

    protected override void DisableFSM() => InitialState();

    // IStateMachine<>
    public void EnterState(PlayerUnitFSM enteringState) {
        state = enteringState;

        // debug
        GetComponentInChildren<TextMeshPro>().SetText(state.ToString());

        switch (state) {
            // when you're entering Idle, it's from being selected
            // therefore, reset your controller's selections
            case PlayerUnitFSM.Idle:
                break;

            // re-calc move range, and display it
            case PlayerUnitFSM.MoveSelection:
                UpdateThreatRange();
                StartCoroutine( Utils.LateFrame(DisplayThreatRange) );
                break;

            case PlayerUnitFSM.Moving:
                break;

            case PlayerUnitFSM.AttackSelection:
                // disable enemy unit controller for a time
                enemyUnitController.ChangeState(EnemyUnitController.ControllerFSM.Inactive);

                UpdateThreatRange(standing: true);
                StartCoroutine( Utils.LateFrame(DisplayThreatRange) );
                break;

            case PlayerUnitFSM.Attacking:
                break;

            case PlayerUnitFSM.PreWait:
                holdTimer.StartTimer(Wait, CancelWait);
                break;
        }
    }

    // IStateMachine<>
    public void ExitState(PlayerUnitFSM exitingState) {
        switch (exitingState) {
            case PlayerUnitFSM.Idle:
                break;

            case PlayerUnitFSM.MoveSelection:
                battleMap.ResetHighlight();
                break;

            case PlayerUnitFSM.Moving:
                if (cancelSignal) {
                    cancelSignal = false;

                    StopAllCoroutines();
                    spriteAnimator.ClearStacks();

                    unitMap.ClearReservation(_reservedGridPosition);
                    UndoMovement();

                } else {
                    unitMap.MoveUnit(this, _reservedGridPosition);
                }
                break;

            case PlayerUnitFSM.AttackSelection:
                // re-enable EnemyUnitController at the end of the frame
                // this is to avoid any same-frame reactivation and Event triggering/listening
                StartCoroutine(
                    Utils.LateFrame(() => {
                        enemyUnitController.ChangeState(EnemyUnitController.ControllerFSM.NoPreview);
                    })
                );

                // disable enemy unit controller for a time
                battleMap.ResetHighlight();
                break;

            case PlayerUnitFSM.Attacking:
                break;

            case PlayerUnitFSM.PreWait:
                break;
        }
        state = PlayerUnitFSM.Idle;
    }

    public void ContextualInteractAt(GridPosition gp) {
        if (!turnActive) return;

        switch (state) {
            ///////////////////////////////////////////////
            // ie Active the unit, go to select movement //
            ///////////////////////////////////////////////
            case PlayerUnitFSM.Idle:
                if (moveAvailable) {
                    ChangeState(PlayerUnitFSM.MoveSelection);
                } else if (attackAvailable) {
                    ChangeState(PlayerUnitFSM.AttackSelection);
                }
                break;

            ////////////////////////////////////////////////////////////////////////
            // The GridPosition gp is the selection made to move the unit towards //
            ////////////////////////////////////////////////////////////////////////
            case PlayerUnitFSM.MoveSelection:
                if (gp == gridPosition) {
                    if (attackAvailable) ChangeState(PlayerUnitFSM.AttackSelection);

                // else if it's a valid movement to be had:
                } else {
                    Path<GridPosition>? pathTo = moveRange.BFS(gridPosition, gp);

                    // if a path exists to the destination, smoothly move along the path
                    // after reaching your destination, officially move via unitMap
                    if (pathTo != null) {
                        StartCoroutine( spriteAnimator.SmoothMovementPath<GridPosition>(pathTo, battleMap) );

                        unitMap.ReservePosition(this, gp);
                        _reservedGridPosition = gp;  // save for ContextualNoInteract to move via unitMap
                        moveAvailable = false;

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
                    // if (moveAvailable) ChangeState(PlayerUnitFSM.MoveSelection);
                    // else, do nothing at all

                } else {

                    // if there's a ValidAttack on the mouseclick'd area
                    if (attackRange.ValidAttack(gp)) {
                        Unit? enemy = unitMap.UnitAt(gp);

                        // if there's an enemy unit at that spot, create and execute an Engagement
                        if (enemy != null) {
                            ChangeState(PlayerUnitFSM.Attacking);
                            attackAvailable = false;

                            engagementResolveFlag = true;
                            Engagement engagement = Engagement.Create(this, enemy);
                            StartCoroutine( engagement.Resolve() );

                            // wait until the engagement has ended
                            // once the engagement has processed, resolve the casualties
                            // once the casualties are resolved, EndTurnSelectedUnit()
                            StartCoroutine(
                                engagement.ExecuteAfterResolving(() => {
                                    engagementResolveFlag = false;
                                })
                            );
                                                            

                        // else, just end your turn for now
                        // by changing state to Attacking, you'll end your turn pretty much immediately
                        } else {
                            ChangeState(PlayerUnitFSM.Idle);
                        }

                    } else {
                        Debug.Log($"No valid attack exists from {gridPosition} to {gp}");
                        ChangeState(PlayerUnitFSM.Idle);
                    }
                }
                break;

            case PlayerUnitFSM.Attacking:
                break;

            case PlayerUnitFSM.PreWait:
                Debug.Log($"This shouldn't even be able to happen?");
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
                if (cancelSignal) {
                    ChangeState(PlayerUnitFSM.Idle);
                    break;
                }

                if (spriteAnimator.isMoving) {    
                    // just spin

                // we've finished moving
                } else {

                    // if there's an in-range enemy, go to AttackSelection
                    // if (attackAvailable && ValidAttackExistsFrom(_reservedGridPosition)) {
                    if (attackAvailable) {
                        ChangeState(PlayerUnitFSM.AttackSelection);

                    // there's no one around to receive your attack, so just end turn
                    } else {
                        Wait();
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

                // wait until simply animating
                if (engagementResolveFlag || spriteAnimator.isMoving || spriteAnimator.isAnimating) {    
                    // just spin

                } else {
                    Wait();
                }
                break;

            case PlayerUnitFSM.PreWait:
                break;
        }
    }

    // this essentially is an "undo" for us
    // undo all the way to Idle
    public override void RevertTurn() {
        switch (state) {
            case PlayerUnitFSM.Moving:
                cancelSignal = true;
                break;

            case PlayerUnitFSM.MoveSelection:
            case PlayerUnitFSM.AttackSelection:
                if (turnActive) {
                    if (moveAvailable == false) UndoMovement();
                    ChangeState(PlayerUnitFSM.Idle);
                }
                break;
        }
    }

    // NOTE: This is janky as hell. Really, I should be using Reservations in the UnitMap, but this kinda works...
    // there theoretically exists a period of time in which things snap around, as MoveUnit can move a Transform, like SpriteAnimator
    // however, the SmoothMovementGrid should override that. I don't know the order of operations vis-a-vis coroutines etc
    private void UndoMovement() {
        // starts from gridPosition
        // StartCoroutine(spriteAnimator.SmoothMovementGrid<GridPosition>(_startingGridPosition, battleMap, _fixedTime: 0.05f));

        // modifies gridPosition & updates threat range
        unitMap.MoveUnit(this, _startingGridPosition);
        _reservedGridPosition = gridPosition;
        RefreshInfo();

        // re-aligns unit's transform without MoveUnit call
        // StartCoroutine(spriteAnimator.ExecuteAfterMoving(() => unitMap.AlignUnit(this, _startingGridPosition) ));
    }

    // this needs to run at the end of the frame
    // this is because of our decoupled event processing
    // basically, the PlayerUnits are displaying  before the enemy units drop the display
    //
    // always display AttackRange first, because it is partially overwritten by MoveRange by definition
    protected override void DisplayThreatRange() {
        if (moveRange == null || attackRange == null) UpdateThreatRange();
        
        attackRange.Display(battleMap);
        moveRange.Display(battleMap);

    	foreach (GridPosition gp in _ThreatenedRange()) {
			if (moveRange.field.ContainsKey(gp)) {
				battleMap.Highlight(gp, Constants.threatColorIndigo);
			}
		}
        // foreach (GridPosition gp in moveRange.field.Keys) {
		// 	if (unitMap.ReservedAt(gp)) {
        //         battleMap.Highlight(gridPosition, Constants.reservedColorBlue);
        //     }
		// }

        battleMap.Highlight(gridPosition, Constants.selectColorWhite);
    }

    private IEnumerable<GridPosition> _ThreatenedRange() {
		HashSet<GridPosition> threatened = new HashSet<GridPosition>();

		foreach (EnemyUnit enemy in enemyUnitController.activeUnits) {
            if (enemy.attackRange == null) enemy.UpdateThreatRange();
			threatened.UnionWith(enemy.attackRange.field.Keys);
		}

		foreach (GridPosition gp in threatened) yield return gp;
	}


    // diff from Unit.FinishTurn: send signal to the parent controller
    public override void FinishTurn() {
        turnActive = false;
        moveAvailable = false;
        attackAvailable = false;
        spriteRenderer.color = new Color(0.75f, 0.75f, 0.75f, 1f);

        playerUnitController.CheckEndPhase();
    }

    public void FinishTurnNoCheck() {
        turnActive = false;
        moveAvailable = false;
        attackAvailable = false;
        spriteRenderer.color = new Color(0.75f, 0.75f, 0.75f, 1f);
    }

    private bool ValidAttackExistsFrom(GridPosition fromPosition) {
        AttackRange standing = AttackRange.Standing(fromPosition, unitStats.MIN_RANGE, unitStats.MAX_RANGE);
        return enemyUnitController.activeUnits.Where(enemy => standing.ValidAttack(enemy.gridPosition)).Any();
    }

    // this is an Action which finishes the unit turn early,
    // / but does so in a way that requires some weird clean up
    public void Wait() {
        FinishTurn();
        ChangeState(PlayerUnitFSM.Idle);
    }
    
    public void ContextualWait() {
        if (turnActive && (moveAvailable || attackAvailable) ) {
            switch (state) {
                case PlayerUnitFSM.Moving:
                case PlayerUnitFSM.Attacking:
                case PlayerUnitFSM.Idle:
                case PlayerUnitFSM.MoveSelection:
                    break;

                // only start detecting the Wait signal if you're not animating, etc
                case PlayerUnitFSM.AttackSelection:
                    ChangeState(PlayerUnitFSM.PreWait);
                    break;
            }
        }
    }

    public void CancelWait() {
        if (state == PlayerUnitFSM.PreWait) {
            holdTimer.CancelTimer();
            
            // since you can only enter PreWait from AttackSelection, head back there
            // it will handle itself wrt going to Idle and checking attackAvailable
            ChangeState(PlayerUnitFSM.AttackSelection);
        }
    }
}
