using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyUnit : Unit, IStateMachine<EnemyUnit.EnemyUnitFSM>
{
    public enum EnemyUnitFSM {
        Idle,
        Preview,
        Moving,
        Attacking
    }
    [SerializeField] private EnemyUnitFSM state = EnemyUnitFSM.Idle;

    void Start() {
        moveRange = new MoveRange(gridPosition);    // empty
        attackRange = new AttackRange(moveRange, unitStats.MIN_RANGE, unitStats.MAX_RANGE);

        EnterState(EnemyUnitFSM.Idle);
    }

    public void ChangeState(EnemyUnitFSM newState) {
        ExitState(state);
        EnterState(newState);
    }

    public void ExitState(EnemyUnitFSM exitingState) {
        Debug.Log($"{this} exiting state {exitingState}");

        switch (exitingState) {
            case EnemyUnitFSM.Idle:
                break;

            case EnemyUnitFSM.Preview:
                moveRange?.ClearDisplay(battleMap);
                attackRange?.ClearDisplay(battleMap);
                break;

            case EnemyUnitFSM.Moving:
                break;

            case EnemyUnitFSM.Attacking:
                break;
        }
        state = EnemyUnitFSM.Idle;
    }

    public void EnterState(EnemyUnitFSM enteringState) {
        Debug.Log($"{this} entering state {enteringState}");
        state = enteringState;

        switch (enteringState) {
            case EnemyUnitFSM.Idle:
                break;

            case EnemyUnitFSM.Preview:
                moveRange = GenerateMoveRange(gridPosition, unitStats.MOVE);
                attackRange = GenerateAttackRange(unitStats.MIN_RANGE, unitStats.MAX_RANGE);

                // always display AttackRange first, because it is partially overwritten by MoveRange by definition
                attackRange.Display(battleMap);
                moveRange.Display(battleMap);
                break;

            case EnemyUnitFSM.Moving:
                break;

            case EnemyUnitFSM.Attacking:
                break;
        }
    }

    public void ContextualInteractAt(GridPosition gp) {
        switch (state) {

            //////////////////////////////////////////////
            // ie Active the unit, go to select preview //
            //////////////////////////////////////////////
            case EnemyUnitFSM.Idle:
                ChangeState(EnemyUnitFSM.Preview);
                break;

            ////////////////////////////////////////////////
            // Any other click will send you back to idle //
            ////////////////////////////////////////////////
            case EnemyUnitFSM.Preview:
                ChangeState(EnemyUnitFSM.Idle);
                break;

            case EnemyUnitFSM.Moving:
                break;

            case EnemyUnitFSM.Attacking:
                break;
        }
    }
}
