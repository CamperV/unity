using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class PlayerUnit : Unit, IStateMachine<PlayerUnit.PlayerUnitFSM>
{
    // additional Component references
    private PlayerUnitController _parentController;
    public PlayerUnitController ParentController { 
        get {
            if (_parentController == null) {
                _parentController = GetComponentInParent<PlayerUnitController>();
            }
            return _parentController;
        }
    }

    public enum PlayerUnitFSM {
        Idle,
        MoveSelection,
        Moving,
        AttackSelection,
        Attacking
    }
    [SerializeField] public PlayerUnitFSM state { get; set; } = PlayerUnitFSM.Idle;

    void Start() {
        // register any relevant events
        EventManager.inst.inputController.RightMouseClickEvent += _ => ChangeState(PlayerUnit.PlayerUnitFSM.Idle);

        moveRange = new MoveRange(gridPosition);    // empty
        attackRange = new AttackRange(moveRange, unitStats.MIN_RANGE, unitStats.MAX_RANGE);

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
                ParentController.ClearInteraction();
                break;

            // re-calc move range, and display it
            case PlayerUnitFSM.MoveSelection:
                moveRange = GenerateMoveRange(gridPosition, unitStats.MOVE);
                attackRange = GenerateAttackRange(unitStats.MIN_RANGE, unitStats.MAX_RANGE);

                // always display AttackRange first, because it is partially overwritten by MoveRange by definition
                attackRange.Display(battleMap);
                moveRange.Display(battleMap);
                battleMap.Highlight(gridPosition, Constants.selectColorWhite);
                break;

            case PlayerUnitFSM.Moving:
                break;

            case PlayerUnitFSM.AttackSelection:
                moveRange = GenerateMoveRange(gridPosition, 0);
                attackRange = GenerateAttackRange(unitStats.MIN_RANGE, unitStats.MAX_RANGE);

                attackRange.Display(battleMap);
                battleMap.Highlight(gridPosition, Constants.selectColorWhite);
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
                unitMap.MoveUnit(this, gridPosition);
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
                        gridPosition = gp;  // save for ContextualNoInteract to move via unitMap

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
                } else {
                    ChangeState(PlayerUnitFSM.AttackSelection);
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
                    ChangeState(PlayerUnitFSM.Idle);
                }
                break;
        }
    }
}
