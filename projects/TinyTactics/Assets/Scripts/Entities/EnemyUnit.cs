using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class EnemyUnit : Unit, IStateMachine<EnemyUnit.EnemyUnitFSM>
{
    public enum EnemyUnitFSM {
        Idle,
        Preview,
        Moving,
        Attacking
    }
    [SerializeField] public EnemyUnitFSM state { get; set; } = EnemyUnitFSM.Idle;

    void Start() {
        // register any relevant events
        EventManager.inst.inputController.RightMouseClickEvent += _ => ChangeState(EnemyUnit.EnemyUnitFSM.Idle);

        moveRange = new MoveRange(gridPosition);    // empty
        attackRange = new AttackRange(moveRange, unitStats.MIN_RANGE, unitStats.MAX_RANGE);

        EnterState(EnemyUnitFSM.Idle);
    }

    public void ChangeState(EnemyUnitFSM newState) {
        if (newState == state) return;

        ExitState(state);
        EnterState(newState);
    }

    public void EnterState(EnemyUnitFSM enteringState) {
        state = enteringState;
        
        // debug
        GetComponentInChildren<TextMeshPro>().SetText(state.ToString());

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

    public void ExitState(EnemyUnitFSM exitingState) {
        Debug.Log($"{this} exiting state {exitingState}");

        switch (exitingState) {
            case EnemyUnitFSM.Idle:
                break;

            case EnemyUnitFSM.Preview:
                battleMap.ResetHighlight();
                break;

            case EnemyUnitFSM.Moving:
                break;

            case EnemyUnitFSM.Attacking:
                break;
        }
        state = EnemyUnitFSM.Idle;
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
