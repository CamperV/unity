using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(EnemyBrain))]
public class EnemyUnit : Unit, IStateMachine<EnemyUnit.EnemyUnitFSM>
{
    // additional components
    private EnemyBrain brain;

    public int Initiative { get => brain.CalculateInitiative(playerUnitController.entities); }

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

    void Update() {
        ContextualNoInteract();
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
                unitMap.MoveUnit(this, _reservedGridPosition);
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
            case EnemyUnitFSM.Attacking:
                break;
        }
    }

    public void ContextualNoInteract() {
        switch (state) {
            case EnemyUnitFSM.Idle:
                break;

            ///////////////////////////////////////////////////////////
            // Every frame that we are moving (after FlowChart),     //
            // check the spriteAnimator. As soon as we stop moving,  //
            // update our position via unitMap and ChangeState       //
            ///////////////////////////////////////////////////////////
            case EnemyUnitFSM.Moving:
                if (spriteAnimator.isMoving) {    
                    // just spin

                // we've finished moving
                } else {
                    ChangeState(EnemyUnitFSM.Idle);
                }
                break;

            ////////////////////////////////////////////////////////////////////
            // Every frame that we're animating our attack (after selecting), //
            // check the spriterAnimator. As soon as we stop animating,       //
            // disable your phase and move into Idle                          //
            ////////////////////////////////////////////////////////////////////
            case EnemyUnitFSM.Attacking:
                if (spriteAnimator.isMoving || spriteAnimator.isAnimating) {    
                    // just spin
                } else {
                    ChangeState(EnemyUnitFSM.Idle);
                }
                break;
        }
    }

    public IEnumerator TakeActionFlowChart() {

        // 1
        brain.RefreshTargets(playerUnitController.entities);
        
        // 2 determine optimal DamagePackage, which determines move and target
        // 2a if no DamagePackages exist... don't do anything
        foreach (EnemyBrain.DamagePackage dp in brain.OptimalDamagePackages()) {
            // Debug.Log($"Got DamagePackage -> {dp.target} [{dp.potentialDamage}] from {dp.fromPosition}");

            Path<GridPosition>? pathTo = moveRange.BFS(gridPosition, dp.fromPosition);

            // if a path exists to the destination, smoothly move along the path
            // after reaching your destination, officially move via unitMap
            if (pathTo != null) {
                StartCoroutine( spriteAnimator.SmoothMovementPath<GridPosition>(pathTo, battleMap) );

                unitMap.ReservePosition(this, dp.fromPosition);
                _reservedGridPosition = dp.fromPosition;  // save for ContextualNoInteract to move via unitMap
                moveAvailable = false;
                ChangeState(EnemyUnitFSM.Moving);

                // WAIT FOR MOVEMENT TO COMPLETE
                yield return new WaitUntil(() => state == EnemyUnitFSM.Idle);
                // WAIT FOR MOVEMENT TO COMPLETE

                Debug.Log($"Now I, {this}, will attack {dp.target} from my gridPosition, {gridPosition}. [{gridPosition == dp.fromPosition}]");
                break;
            }
        }

        // after you've attacked, finish your turn
        FinishTurn();
    }

    // this essentially is an "undo" for us
    // undo all the way to Idle
    public override void Cancel() {
        if (state == EnemyUnitFSM.Idle) return;
        ChangeState(EnemyUnitFSM.Idle);
    }

    protected override void DisplayThreatRange() {
        if (moveRange == null || attackRange == null) UpdateThreatRange();

        attackRange.Display(battleMap);
        moveRange.Display(battleMap, Constants.threatColorYellow);
        battleMap.Highlight(gridPosition, Constants.selectColorWhite);
    }
}
