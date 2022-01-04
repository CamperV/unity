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

    // if move selection is done with a middle-click, immediately Wait upon finish moving
    private bool auxiliaryInteractFlag = false;

    private GridPosition _previousMouseOver; // for MoveSelection and AttackSelection (ContextualNoInteract)
    private Path<GridPosition>? pathToMouseOver;

    void Update() => ContextualNoInteract();

    // IStateMachine<>
    public void ChangeState(PlayerUnitFSM newState) {
        if (newState == state) return;
        
        ExitState(state);
        EnterState(newState);
    }
    
    protected override void Start() {
        base.Start();
        InitialState();
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
        debugStateLabel.SetText(state.ToString());

        switch (state) {
            // when you're entering Idle, it's from being selected
            // therefore, reset your controller's selections
            case PlayerUnitFSM.Idle:
                break;

            // re-calc move range, and display it
            case PlayerUnitFSM.MoveSelection:
                UpdateThreatRange();
                StartCoroutine( Utils.LateFrame(DisplayThreatRange) );

                //
                UIManager.inst.EnableUnitDetail(this);
                break;

            case PlayerUnitFSM.Moving:
                break;

            case PlayerUnitFSM.AttackSelection:
                UpdateThreatRange(standing: true);
                StartCoroutine( Utils.LateFrame(DisplayThreatRange) );
                break;

            case PlayerUnitFSM.Attacking:
                break;

            case PlayerUnitFSM.PreWait:
                holdTimer.StartTimer(
                    () => {
                        Wait();
                        personalAudioFX.PlaySpecialInteractFX();
                    },
                    CancelWait
                );
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
                battleMap.ClearDisplayPath();
                //
                UIManager.inst.DisableUnitDetail();
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
                // disable enemy unit controller for a time
                battleMap.ResetHighlight();
                
                UIManager.inst.DisableEngagementPreview();
                break;

            case PlayerUnitFSM.Attacking:
                break;

            case PlayerUnitFSM.PreWait:
                break;
        }
        state = PlayerUnitFSM.Idle;
    }

    public void ContextualInteractAt(GridPosition gp, bool auxiliaryInteract) {
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

                if (moveAvailable || attackAvailable) personalAudioFX.PlayInteractFX();
                break;

            ////////////////////////////////////////////////////////////////////////
            // The GridPosition gp is the selection made to move the unit towards //
            ////////////////////////////////////////////////////////////////////////
            case PlayerUnitFSM.MoveSelection:
                if (gp == gridPosition) {
                    if (attackAvailable) {
                        ChangeState(PlayerUnitFSM.AttackSelection);
                        //
                        personalAudioFX.PlayInteractFX();
                    }

                // else if it's a valid movement to be had:
                } else {

                    // pathToMouseOver is updated right before this in ContextualNoUpdate
                    // if a path exists to the destination, smoothly move along the path
                    // after reaching your destination, officially move via unitMap
                    if (pathToMouseOver != null) {
                        StartCoroutine(
                            spriteAnimator.SmoothMovementPath<GridPosition>(pathToMouseOver, battleMap)
                        );

                        unitMap.ReservePosition(this, gp);
                        _reservedGridPosition = gp;  // save for ContextualNoInteract to move via unitMap
                        moveAvailable = false;

                        ChangeState(PlayerUnitFSM.Moving);
                        auxiliaryInteractFlag = auxiliaryInteract;

                        FireOnMoveEvent(pathToMouseOver);

                        //
                        personalAudioFX.PlayInteractFX();
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
                    if (attackRange.ValidAttack(gp) && EnemyAt(gp) != null) {
                        EnemyUnit? enemy = EnemyAt(gp);

                        // if there's an enemy unit at that spot, create and execute an Engagement
                        ChangeState(PlayerUnitFSM.Attacking);

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
                        
                        // NOTE: when attacking normally, consume movement
                        // Re-movement only granted with certain perks
                        moveAvailable = false;
                        attackAvailable = false;
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

            // this is where we constantly recalculate/show the path to your mouse destination
            case PlayerUnitFSM.MoveSelection:

                // when the mouse-on-grid changes:
                if (battleMap.CurrentMouseGridPosition != _previousMouseOver) {
                    battleMap.ClearDisplayPath();

                    if (battleMap.MouseInBounds) {
                        pathToMouseOver = moveRange.BFS(gridPosition, battleMap.CurrentMouseGridPosition);
                        _previousMouseOver = battleMap.CurrentMouseGridPosition;

                        if (pathToMouseOver != null) battleMap.DisplayPath(pathToMouseOver);
                    }
                }
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

                    // if this interact was fired via Middle-Mouse, immediately wait
                    if (auxiliaryInteractFlag) {
                        auxiliaryInteractFlag = false;
                        Wait();

                    // if there's an in-range enemy, go to AttackSelection
                    // if (attackAvailable && ValidAttackExistsFrom(_reservedGridPosition)) {
                    } else if (attackAvailable) {
                        ChangeState(PlayerUnitFSM.AttackSelection);

                    // there's no one around to receive your attack, so just end turn
                    } else {
                        Wait();
                    }
                }
                break;

            case PlayerUnitFSM.AttackSelection:

                // when the mouse-on-grid changes:
                if (battleMap.CurrentMouseGridPosition != _previousMouseOver) {
                    _previousMouseOver = battleMap.CurrentMouseGridPosition;

                    // reset these
                    DisplayThreatRange();
                    UIManager.inst.DisableEngagementPreview();

                    if (attackRange.ValidAttack(battleMap.CurrentMouseGridPosition) && EnemyAt(battleMap.CurrentMouseGridPosition) != null) {
                        EnemyUnit? enemy = EnemyAt(battleMap.CurrentMouseGridPosition);
                        battleMap.Highlight(battleMap.CurrentMouseGridPosition, Constants.threatColorYellow);

                        // create and display EngagementPreviews here
                        UIManager.inst.EnableEngagementPreview( Engagement.Create(this, enemy), enemy.transform );
                    }
                }
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

                    // if somehow you're able to attack without moving:
                    // > Perk: AfterImage
                    if (moveAvailable) {
                        ChangeState(PlayerUnitFSM.MoveSelection);
                    } else {
                        Wait();
                    }
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

        // modifies gridPosition & updates threat range
        unitMap.MoveUnit(this, _startingGridPosition);
        _reservedGridPosition = gridPosition;
        buffManager.RemoveAllMovementBuffs();
        RefreshInfo();
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

        FireOnFinishTurnEvent();
        playerUnitController.CheckEndPhase();
    }

    public void FinishTurnNoCheck() {
        turnActive = false;
        moveAvailable = false;
        attackAvailable = false;
        spriteRenderer.color = new Color(0.75f, 0.75f, 0.75f, 1f);

        FireOnFinishTurnEvent();
    }

    private bool ValidAttackExistsFrom(GridPosition fromPosition) {
        AttackRange standing = AttackRange.Standing(fromPosition, equippedWeapon.weaponStats.MIN_RANGE, equippedWeapon.weaponStats.MAX_RANGE);
        return enemyUnitController.activeUnits.Where(enemy => standing.ValidAttack(enemy.gridPosition)).Any();
    }

    // this is an Action which finishes the unit turn early,
    // / but does so in a way that requires some weird clean up
    public void Wait() {
        FinishTurn();
        ChangeState(PlayerUnitFSM.Idle);
    }

    public void WaitNoCheck() {
        FinishTurnNoCheck();
        ChangeState(PlayerUnitFSM.Idle);
    }
    
    public void ContextualWait() {
        if (turnActive) {
            if (attackAvailable || (moveAvailable && attackAvailable)) {
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
            } else if (moveAvailable && !attackAvailable) {
                switch (state) {
                    case PlayerUnitFSM.Moving:
                    case PlayerUnitFSM.Attacking:
                    case PlayerUnitFSM.Idle:
                        break;

                    // only start detecting the Wait signal if you're not animating, etc
                    case PlayerUnitFSM.MoveSelection:
                        ChangeState(PlayerUnitFSM.PreWait);
                        break;

                    case PlayerUnitFSM.AttackSelection:
                        break;
                }
            }
        }
    }

    public void CancelWait() {
        if (state == PlayerUnitFSM.PreWait) {
            holdTimer.CancelTimer();
            
            // since you can only enter PreWait from AttackSelection, head back there
            // it will handle itself wrt going to Idle and checking attackAvailable
            if (moveAvailable) {
                ChangeState(PlayerUnitFSM.MoveSelection);

            } else {
                ChangeState(PlayerUnitFSM.AttackSelection);
            }
        }
    }

    private EnemyUnit? EnemyAt(GridPosition gp) {
        Unit? unit = unitMap.UnitAt(gp);
        if (unit != null && unit.GetType() == typeof(EnemyUnit))
            return (unit as EnemyUnit);
        else return null;
    }
}
