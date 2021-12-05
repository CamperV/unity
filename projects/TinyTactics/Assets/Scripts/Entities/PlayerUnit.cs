using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(SpriteAnimator))]
public abstract class PlayerUnit : GridEntity, IStateMachine<PlayerUnit.PlayerUnitFSM>
{
    public enum PlayerUnitFSM {
        Idle,
        MoveSelection,
        AttackSelection
    }
    [SerializeField] private PlayerUnitFSM state = PlayerUnitFSM.Idle;
    
    // necessary references
    private GridEntityMap gridEntityMap;
    private BattleMap battleMap;

    // other
    private SpriteAnimator spriteAnimator;
    private Pathfinder<GridPosition> pathfinder;

    private MoveRange moveRange;
    // private AttackRange<GridPosition> attackRange;


    void Awake() {
        gridEntityMap = GetComponentInParent<GridEntityMap>();
        battleMap = gridEntityMap.GetComponentInChildren<BattleMap>();
        spriteAnimator = GetComponent<SpriteAnimator>();
    }

    void Start() {
        pathfinder = new Pathfinder<GridPosition>(battleMap);

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

            // re-calc move range
            case PlayerUnitFSM.MoveSelection:
                moveRange = pathfinder.GenerateFlowField<MoveRange>(gridPosition, range: unitStats.MOVE);
                moveRange.Display(battleMap);
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
                    Path<GridPosition>? pathTo = pathfinder.BFS(gridPosition, gp);

                    // if a path exists to the destination, smoothly move along the path
                    // after reaching your destination, officially move via GridEntityMap
                    if (pathTo != null) {
                        StartCoroutine( spriteAnimator.SmoothMovementPath<GridPosition>(pathTo, battleMap) );
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
}
