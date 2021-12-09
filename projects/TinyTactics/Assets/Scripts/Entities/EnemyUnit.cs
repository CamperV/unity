using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyUnit : Unit, IStateMachine<EnemyUnit.EnemyUnitFSM>
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

        moveRange = null;
        attackRange = null;
        EnterState(EnemyUnitFSM.Idle);
    }

    public void ChangeState(EnemyUnitFSM newState) {
        if (newState == state) return;

        ExitState(state);
        EnterState(newState);
    }

    public void EnterState(EnemyUnitFSM enteringState) {
        Debug.Log($"{this} entering {enteringState}");
        state = enteringState;
        
        // debug
        GetComponentInChildren<TextMeshPro>().SetText(state.ToString());

        switch (enteringState) {
            // when you're entering Idle, it's from being selected
            // therefore, reset your controller's selections
            case EnemyUnitFSM.Idle:
                enemyUnitController.ClearSelection();
                break;

            case EnemyUnitFSM.Preview:
                UpdateThreatRange();
                StartCoroutine( Utils.LateFrame(DisplayThreatRange) );
                break;

            case EnemyUnitFSM.Moving:
            case EnemyUnitFSM.Attacking:
                break;
        }
    }

    public void ExitState(EnemyUnitFSM exitingState) {
        Debug.Log($"{this} exiting {exitingState}");
        switch (exitingState) {
            case EnemyUnitFSM.Idle:
                break;

            case EnemyUnitFSM.Preview:
                battleMap.ResetHighlight();
                break;

            case EnemyUnitFSM.Moving:
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

    // this essentially is an "undo" for us
    // undo all the way to Idle
    public void Cancel() {
        if (state == EnemyUnitFSM.Idle) return;
        ChangeState(EnemyUnitFSM.Idle);
    }

    protected void DisplayThreatRange() {
        attackRange.Display(battleMap);
        moveRange.Display(battleMap, Constants.threatColorYellow);
        battleMap.Highlight(gridPosition, Constants.selectColorWhite);
    }
}
