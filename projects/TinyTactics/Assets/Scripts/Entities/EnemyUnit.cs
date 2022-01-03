using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(EnemyBrain))]
public class EnemyUnit : Unit, IStateMachine<EnemyUnit.EnemyUnitFSM>
{
    // additional components
    private EnemyBrain brain;

    public int Initiative => brain.CalculateInitiative(playerUnitController.activeUnits);

    // IStateMachine<>
    public enum EnemyUnitFSM {
        Idle,
        Preview,
        Moving,
        Attacking
    }
    [SerializeField] public EnemyUnitFSM state { get; set; } = EnemyUnitFSM.Idle;

    private bool engagementResolveFlag = false;

    protected override void Awake() {
        base.Awake();
        brain = GetComponent<EnemyBrain>();
    }

    protected override void Start() {
        base.Start();
        InitialState();
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

    protected override void DisableFSM() => InitialState();

    // IStateMachine<>
    public void EnterState(EnemyUnitFSM enteringState) {
        state = enteringState;
        
        // debug
        debugStateLabel.SetText(state.ToString());

        switch (enteringState) {
            // when you're entering Idle, it's from being selected
            // therefore, reset your controller's selections
            case EnemyUnitFSM.Idle:
                enemyUnitController.ClearPreview();
                break;

            case EnemyUnitFSM.Preview:
                UpdateThreatRange();
                StartCoroutine( Utils.LateFrame(DisplayThreatRange) );
                //
                UIManager.inst.EnableUnitDetail(this);
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
                //
                UIManager.inst.DisableUnitDetail();
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
                if (gp == gridPosition) ChangeState(EnemyUnitFSM.Preview);
                break;

            ////////////////////////////////////////////////
            // Any other click will send you back to idle //
            ////////////////////////////////////////////////
            case EnemyUnitFSM.Preview:
                if (gp != gridPosition) ChangeState(EnemyUnitFSM.Idle);
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
                if (engagementResolveFlag || spriteAnimator.isMoving || spriteAnimator.isAnimating) {    
                    // just spin

                } else {
                    ChangeState(EnemyUnitFSM.Idle);
                }
                break;
        }
    }

    public IEnumerator TakeActionFlowChart() {
        // 1
        brain.RefreshTargets(playerUnitController.activeUnits);
        
        // 2 determine optimal DamagePackage, which determines move and target
        // 2a if no DamagePackages exist... don't do anything
        EnemyBrain.DamagePackage? selectedDmgPkg = null;
        Path<GridPosition>? pathTo = null;

        foreach (EnemyBrain.DamagePackage candidateDmgPkg in brain.OptimalDamagePackages()) {
            Debug.Log($"Got DamagePackage -> {candidateDmgPkg.target} [{candidateDmgPkg.potentialDamage}] from {candidateDmgPkg.fromPosition}");

            pathTo = moveRange.BFS(gridPosition, candidateDmgPkg.fromPosition);

            // if a path exists to the destination, smoothly move along the path
            // after reaching your destination, officially move via unitMap
            if (pathTo != null) {
                selectedDmgPkg = candidateDmgPkg;
                break;
            }
        }
        if (pathTo == null) {
            Debug.Log($"Finishing turn early, {this} can't find a path to anything it wants to do.");
            FinishTurn();
            yield break;
        }

        //
        // execute movement portion
        StartCoroutine( spriteAnimator.SmoothMovementPath<GridPosition>(pathTo, battleMap) );

        unitMap.ReservePosition(this, selectedDmgPkg.Value.fromPosition);
        _reservedGridPosition = selectedDmgPkg.Value.fromPosition;  // save for ContextualNoInteract to move via unitMap
        moveAvailable = false;
        ChangeState(EnemyUnitFSM.Moving);

        // WAIT FOR MOVEMENT TO COMPLETE
        yield return new WaitUntil(() => state == EnemyUnitFSM.Idle);
        // WAIT FOR MOVEMENT TO COMPLETE

        Engagement engagement = Engagement.Create(this, selectedDmgPkg.Value.target);
        attackAvailable = false;
        ChangeState(EnemyUnitFSM.Attacking);
        //
        engagementResolveFlag = true;
        StartCoroutine( engagement.Resolve() );
        StartCoroutine(
            engagement.ExecuteAfterResolving(() => {
                engagementResolveFlag = false;
            })
        );

        // WAIT FOR ENGAGEMENT TO RESOLVE
        yield return new WaitUntil(() => state == EnemyUnitFSM.Idle);
        // WAIT FOR ENGAGEMENT TO RESOLVE

        // after you've attacked, finish your turn
        FinishTurn();
    }

    // this essentially is an "undo" for us
    // undo all the way to Idle
    public override void RevertTurn() {
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
