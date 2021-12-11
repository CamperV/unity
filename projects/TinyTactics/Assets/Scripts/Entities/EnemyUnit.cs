using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(EnemyBrain))]
public class EnemyUnit : Unit, IStateMachine<EnemyUnit.EnemyUnitFSM>
{
    // additional components
    private EnemyBrain brain;

    private int _Initiative;
    public int Initiative {
        get {
            if (_Initiative == null) _Initiative = brain.CalculateInitiative();
            return _Initiative;
        }
    }

    // IStateMachine<>
    public enum EnemyUnitFSM {
        Idle,
        Preview,
        Moving,
        Attacking
    }
    [SerializeField] public EnemyUnitFSM state { get; set; } = EnemyUnitFSM.Idle;

    protected override void Awake() {
        base.Awake();
        brain = GetComponent<EnemyBrain>();
    }

    void Start() {
        // register any relevant events
        EventManager.inst.inputController.RightMouseClickEvent += at => Cancel();
        
        originalColor = spriteRenderer.color;
        moveRange = null;
        attackRange = null;
        EnterState(EnemyUnitFSM.Idle);
    }

    // IStateMachine<>
    public void ChangeState(EnemyUnitFSM newState) {
        if (newState == state) return;

        ExitState(state);
        EnterState(newState);
    }

    // IStateMachine<> 
    public void InitialState() {
        ExitState(state);
        EnterState(EnemyUnitFSM.Idle);
    }

    // IStateMachine<>
    public void EnterState(EnemyUnitFSM enteringState) {
        state = enteringState;
        
        // debug
        GetComponentInChildren<TextMeshPro>().SetText(state.ToString());

        switch (enteringState) {
            // when you're entering Idle, it's from being selected
            // therefore, reset your controller's selections
            case EnemyUnitFSM.Idle:
                enemyUnitController.ClearPreview();
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

    // IStateMachine<>
    public void ExitState(EnemyUnitFSM exitingState) {
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
            case EnemyUnitFSM.Attacking:
                break;
        }
    }

    public void TakeActionFlowChart() {
        Debug.Log($"{this} is taking action!");
        FinishTurn();

        // move to
            // now, find a full path to the location
            // even if we can't reach it, just go as far as you can
            // CAVEAT: we can't just clip it
            // if we do, we can have enemies standing in the same place.
            // instead, we have to do the laborious thing, and REpath-find to the new clipped position
            //
            // Debug.Log($"{subject}@{subject.gridPosition} found {optimalPosition} to attack {target}@{target.gridPosition}");
            // Path pathToTarget = new UnitPathfinder(subject.obstacles).BFS<Path>(subject.gridPosition, optimalPosition);
            // pathToTarget.Clip(subject.moveRange);

        // attack at
            // if (subject.OptionActive("Attack") && subject.attackRange.ValidAttack(subject, target.gridPosition)) {
            // subject.SetOption("Attack", false);
            // Engagement engagement = new Engagement(subject, target);

            // // wait until the engagement has ended
            // StartCoroutine(engagement.ResolveResults());
            // while (!engagement.resolved) { yield return null; }

            // // wait until results have killed units, if necessary
            // StartCoroutine(engagement.results.ResolveCasualties());
            // while (!engagement.results.resolved) { yield return null; }

        // finally:
        // this will discolor the unit and set its options to false, after movement is complete
        // BUT, don't let the other units move until this subject has finished
            // subject.OnEndTurn();
            // yield return null;
    }

    // this essentially is an "undo" for us
    // undo all the way to Idle
    public override void Cancel() {
        if (state == EnemyUnitFSM.Idle) return;
        ChangeState(EnemyUnitFSM.Idle);
    }

    protected override void DisplayThreatRange() {
        attackRange.Display(battleMap);
        moveRange.Display(battleMap, Constants.threatColorYellow);
        battleMap.Highlight(gridPosition, Constants.selectColorWhite);
    }
}
