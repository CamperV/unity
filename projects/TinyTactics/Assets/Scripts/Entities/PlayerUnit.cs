using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(SpriteAnimator))]
[RequireComponent(typeof(EntityPathfinder))]
[RequireComponent(typeof(UnitStats))]
public abstract class PlayerUnit : GridEntity, IStateMachine<PlayerUnit.PlayerUnitFSM>
{
    public enum PlayerUnitFSM {
        Idle,
        MoveSelection,
        Moving,
        AttackSelection,
        Attacking
    }
    [SerializeField] private PlayerUnitFSM state = PlayerUnitFSM.Idle;
    
    // necessary references
    private GridEntityMap gridEntityMap;
    private BattleMap battleMap;
    private SpriteAnimator spriteAnimator;
    private EntityPathfinder mapPathfinder;
    private UnitStats unitStats;

    // other
    private MoveRange moveRange;
    private AttackRange attackRange;

    void Awake() {
        spriteAnimator = GetComponent<SpriteAnimator>();
        mapPathfinder = GetComponent<EntityPathfinder>();
        unitStats = GetComponent<UnitStats>();

        Battle _topBattleRef = GetComponentInParent<Battle>();
        gridEntityMap        = _topBattleRef.GetComponent<GridEntityMap>();
        battleMap            = _topBattleRef.GetComponentInChildren<BattleMap>();
    }

    void Start() {
        moveRange = new MoveRange(gridPosition);    // empty
        attackRange = new AttackRange(moveRange, unitStats.MIN_RANGE, unitStats.MAX_RANGE);

        EnterState(PlayerUnitFSM.Idle);
    }

    void Update() {
        ContextualNoInteract();
    }

    public void ChangeState(PlayerUnitFSM newState) {
        ExitState(state);
        EnterState(newState);
    }

    public void ExitState(PlayerUnitFSM exitingState) {
        Debug.Log($"{this} exiting state {exitingState}");

        switch (exitingState) {
            case PlayerUnitFSM.Idle:
                break;

            case PlayerUnitFSM.MoveSelection:
                moveRange?.ClearDisplay(battleMap);
                attackRange?.ClearDisplay(battleMap);
                break;

            case PlayerUnitFSM.Moving:
                gridEntityMap.MoveEntity(this, gridPosition);
                break;

            case PlayerUnitFSM.AttackSelection:
                attackRange?.ClearDisplay(battleMap);   
                break;

            case PlayerUnitFSM.Attacking:
                break;
        }
        state = PlayerUnitFSM.Idle;
    }

    public void EnterState(PlayerUnitFSM enteringState) {
        Debug.Log($"{this} entering state {enteringState}");
        state = enteringState;

        switch (state) {
            case PlayerUnitFSM.Idle:
                break;

            // re-calc move range, and display it
            case PlayerUnitFSM.MoveSelection:
                moveRange = RegenerateMoveRange(gridPosition, unitStats.MOVE);
                attackRange = RegenerateAttackRange(unitStats.MIN_RANGE, unitStats.MAX_RANGE);

                // always display AttackRange first, because it is partially overwritten by MoveRange by definition
                attackRange.Display(battleMap);
                moveRange.Display(battleMap);
                break;

            case PlayerUnitFSM.Moving:
                break;

            case PlayerUnitFSM.AttackSelection:
                moveRange = RegenerateMoveRange(gridPosition, 0);
                attackRange = RegenerateAttackRange(unitStats.MIN_RANGE, unitStats.MAX_RANGE);

                attackRange.Display(battleMap);
                break;

            case PlayerUnitFSM.Attacking:
                break;
        }
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
                    // after reaching your destination, officially move via GridEntityMap
                    if (pathTo != null) {
                        StartCoroutine( spriteAnimator.SmoothMovementPath<GridPosition>(pathTo, battleMap) );
                        gridPosition = gp;  // save for ContextualNoInteract to move via gridEntityMap
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
            // update our position via gridEntityMap and ChangeState //
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

            case PlayerUnitFSM.Attacking:
                if (spriteAnimator.isMoving || spriteAnimator.isAnimating) {    
                    // just spin
                } else {
                    ChangeState(PlayerUnitFSM.Idle);
                }
                break;
        }
    }

    private MoveRange RegenerateMoveRange(GridPosition gp, int range) {
        return mapPathfinder.GenerateFlowField<MoveRange>(gp, range: range);
    }

    private AttackRange RegenerateAttackRange(int minRange, int maxRange) {
        return new AttackRange(moveRange, minRange, maxRange);
    }
}
