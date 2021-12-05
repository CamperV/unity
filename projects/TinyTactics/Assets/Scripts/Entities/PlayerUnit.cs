using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(SpriteAnimator))]
public abstract class PlayerUnit : GridEntity, IStateMachine<PlayerUnit.PlayerUnitFSM>
{
    public enum PlayerUnitFSM {
        Idle,
        MoveSelection,
        Moving,
        AttackSelection
    }
    [SerializeField] private PlayerUnitFSM state = PlayerUnitFSM.Idle;
    
    // necessary references
    private GridEntityMap gridEntityMap;
    private BattleMap battleMap;

    // other
    private SpriteAnimator spriteAnimator;
    private Pathfinder<GridPosition> mapPathfinder;

    private MoveRange moveRange;
    // private AttackRange<GridPosition> attackRange;


    void Awake() {
        gridEntityMap = GetComponentInParent<GridEntityMap>();
        battleMap = gridEntityMap.GetComponentInChildren<BattleMap>();
        spriteAnimator = GetComponent<SpriteAnimator>();
    }

    void Start() {
        mapPathfinder = new Pathfinder<GridPosition>(battleMap);
        moveRange = new MoveRange(gridPosition);    // empty

        EnterState(PlayerUnitFSM.Idle);
    }

    public void ChangeState(PlayerUnitFSM newState) {
        ExitState(state);
        EnterState(newState);
    }

    public void ExitState(PlayerUnitFSM exitingState) {
        Debug.Log($"{this} exiting state {exitingState}");

        switch (exitingState) {
            case PlayerUnitFSM.Idle:
            case PlayerUnitFSM.MoveSelection:
                moveRange?.ClearDisplay(battleMap);
                break;

            case PlayerUnitFSM.Moving:
            case PlayerUnitFSM.AttackSelection:
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
                moveRange.Display(battleMap);
                break;

            case PlayerUnitFSM.Moving:
                break;

            case PlayerUnitFSM.AttackSelection:
                // attackRange = new AttackRange(moveRange, attackable);
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
                    ChangeState(PlayerUnitFSM.Idle);

                // else if it's a valid movement to be had:
                } else {
                    Path<GridPosition>? pathTo = new MoveRangePathfinder(moveRange).BFS(gridPosition, gp);

                    // if a path exists to the destination, smoothly move along the path
                    // after reaching your destination, officially move via GridEntityMap
                    if (pathTo != null) {
                        Debug.Log($"Found a path from {gridPosition} to {gp}");
                        StartCoroutine( spriteAnimator.SmoothMovementPath<GridPosition>(pathTo, battleMap) );
                        ChangeState(PlayerUnitFSM.Moving);

                        StartCoroutine( spriteAnimator.ExecuteAfterMoving( () => {
                            gridEntityMap.MoveEntity(this, gp);
                            ChangeState(PlayerUnitFSM.AttackSelection);   
                        }));
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
                    Debug.Log($"{this} would like to try to attack {gp}");
                    ChangeState(PlayerUnitFSM.Idle);
                }
                break;
        }
    }

	// public virtual void DisplayThreatRange() {
	// 	var grid = Battle.active.grid;
	// 	moveRange?.ClearDisplay(grid);
	// 	attackRange?.ClearDisplay(grid);

	// 	UpdateThreatRange();
	// 	attackRange.Display(grid);
	// 	moveRange.Display(grid);

	// 	// add the lil selection square
	// 	grid.UnderlayAt(gridPosition, Constants.selectColorWhite);
	// }

    private MoveRange RegenerateMoveRange(GridPosition gp, int range) {
        return mapPathfinder.GenerateFlowField<MoveRange>(gp, range: range);
    }
}
