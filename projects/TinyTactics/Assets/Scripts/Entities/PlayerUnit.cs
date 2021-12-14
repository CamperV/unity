using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUnit : Unit, IStateMachine<PlayerUnit.PlayerUnitFSM>
{
    // IStateMachine<>
    public enum PlayerUnitFSM {
        Idle,
        MoveSelection,
        Moving,
        AttackSelection,
        Attacking
    }
    [SerializeField] public PlayerUnitFSM state { get; set; } = PlayerUnitFSM.Idle;

    public bool cancelSignal = false;
    private EngagementResults attackResults;
    private bool awaitResults = false;

    void Start() {
        // register any relevant events
        EventManager.inst.inputController.RightMouseClickEvent += at => Cancel();

        originalColor = spriteRenderer.color;
        moveRange = null;
        attackRange = null;
        EnterState(PlayerUnitFSM.Idle);
    }

    void Update() {
        ContextualNoInteract();
    }

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

    // IStateMachine<>
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
                break;

            case PlayerUnitFSM.AttackSelection:
                // disable enemy unit controller for a time
                enemyUnitController.ChangeState(EnemyUnitController.ControllerFSM.Inactive);

                UpdateThreatRange(standing: true);
                StartCoroutine( Utils.LateFrame(DisplayThreatRange) );
                break;

            case PlayerUnitFSM.Attacking:
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
                unitMap.MoveUnit(this, _reservedGridPosition);
                break;

            case PlayerUnitFSM.AttackSelection:
                // re-enable EnemyUnitController at the end of the frame
                // this is to avoid any same-frame reactivation and Event triggering/listening
                StartCoroutine(
                    Utils.LateFrame(() => {
                        enemyUnitController.ChangeState(EnemyUnitController.ControllerFSM.NoPreview);
                    })
                );

                battleMap.ResetHighlight();  
                break;

            case PlayerUnitFSM.Attacking:
                awaitResults = false;
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
                    ChangeState(PlayerUnitFSM.Idle);

                } else {

                    // if there's a ValidAttack (valid tile to attack)
                    if (attackRange.ValidAttack(gp)) {
                        Unit? enemy = unitMap.UnitAt(gp);

                        // if there's an enemy unit at that spot, create and execute an Engagement
                        if (enemy != null) {
                            Engagement engagement = Engagement.Create(this, enemy);
                            StartCoroutine( engagement.Resolve() );

                            // wait until the engagement has ended
                            // once the engagement has processed, resolve the casualties
                            // once the casualties are resolved, EndTurnSelectedUnit()
                            StartCoroutine(
                                engagement.ExecuteAfterResolving(() => {
                                    attackAvailable = false;
                                    ChangeState(PlayerUnitFSM.Attacking);
                                })
                            );
                                                            

                        // else, just end your turn for now
                        // by changing state to Attacking, you'll end your turn pretty much immediately
                        } else {
                            ChangeState(PlayerUnitFSM.Attacking);
                        }

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
                        if (attackAvailable) ChangeState(PlayerUnitFSM.AttackSelection);
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
                if (spriteAnimator.isMoving || spriteAnimator.isAnimating) {    
                    // just spin

                } else {
                    if (cancelSignal) {
                        cancelSignal = false;
                        ChangeState(PlayerUnitFSM.Idle);
                    } else {
                        FinishTurn();
                        ChangeState(PlayerUnitFSM.Idle);
                    }
                }
                break;
        }
    }

    // this essentially is an "undo" for us
    // undo all the way to Idle
    public override void Cancel() {
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
    protected override void DisplayThreatRange() {
        if (moveRange == null || attackRange == null) UpdateThreatRange();
        
        attackRange.Display(battleMap);
        moveRange.Display(battleMap);

    	foreach (GridPosition gp in _ThreatenedRange()) {
			if (moveRange.field.ContainsKey(gp)) {
				battleMap.Highlight(gp, Constants.threatColorIndigo);
			}
		}

        battleMap.Highlight(gridPosition, Constants.selectColorWhite);
    }

    private IEnumerable<GridPosition> _ThreatenedRange() {
		HashSet<GridPosition> threatened = new HashSet<GridPosition>();

		foreach (EnemyUnit enemy in enemyUnitController.entities) {
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
}
