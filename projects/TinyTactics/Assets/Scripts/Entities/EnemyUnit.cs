using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(EnemyBrain))]
public class EnemyUnit : Unit, IStateMachine<EnemyUnit.EnemyUnitFSM>
{
    // additional components
    private EnemyBrain brain;
    public BrainPod assignedPod;
    public int Initiative => brain.CalculateInitiative();

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

                //
                personalAudioFX.PlayInteractFX();
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

    public void RefreshTargets() => brain.RefreshTargets(playerUnitController.activeUnits);

    public void SelectDamagePackage(out EnemyBrain.DamagePackage? selectedDmgPkg, out Path<GridPosition>? pathTo) {
        selectedDmgPkg = null;
        pathTo = null;

        // 1 - refresh targets (done elsewhere)
        
        // 2 determine optimal DamagePackage, which determines move and target
        // 2a if no DamagePackages exist... don't do anything
        foreach (EnemyBrain.DamagePackage candidateDmgPkg in brain.OptimalDamagePackagesInRange(moveRange, attackRange)) {
            pathTo = moveRange.BFS(gridPosition, candidateDmgPkg.fromPosition);

            // if a path exists to the destination, smoothly move along the path
            // after reaching your destination, officially move via unitMap
            if (pathTo != null) {
                selectedDmgPkg = candidateDmgPkg;

                // since they are in the best order possible, break when you get your first candidate
                return;
            }
        }

        // if you've made it this far, that means that there are no DamagePackages in your attack range
        // if you're in a BrainPod, use their shared detection range
        if (assignedPod != null) {
			MoveRange podMoveRange = unitPathfinder.GenerateFlowField<MoveRange>(gridPosition, assignedPod.sharedMoveRangeDimensions);
            AttackRange podAttackRange = AttackRange.OfDimension(gridPosition, assignedPod.sharedAttackRangeDimensions);

            foreach (EnemyBrain.DamagePackage candidateDmgPkg in brain.OptimalDamagePackagesInRange(podMoveRange, podAttackRange)) {
                Path<GridPosition> fullPathTo = podMoveRange.BFS(gridPosition, candidateDmgPkg.fromPosition);

                // decide if this DamagePackage is for you
                if (fullPathTo == null) continue;

                // mask the fullPathTo with your moveRange
                // this is done so that a Pod unit doesn't just use someone else's movement
                // clip the pathTo using your ACTUAL moveRange
                // now... this new path hasn't check the things it needs to, such as "Can I stand here?"
                // that question is asked by unitMap.CanMoveInto, during moveRange creation
                // so it is raised when we ask for a ValidMove ("moveRange.RegisterValidMoveToFunc(unitMap.CanMoveInto);")
                Path<GridPosition> maskedPathTo = Path<GridPosition>.MaskWithinRange<GridPosition>(fullPathTo, moveRange);

                // first, ask that question, and if it works... then it simply works. Return early
                if (moveRange.ValidMoveTo(maskedPathTo.End)) {
                    pathTo = maskedPathTo;
                    selectedDmgPkg = candidateDmgPkg;
                    return;
                }

                // If this unit can't move into the path's end, re-adjust where we're going, and path again.
                // this is done by checking "concentric circles" around the original fullPathTo.End point
                // basically, keep checking until we have a valid path to a square in our moveRange
                // if you never find one... check the next DamagePackage
                // if THAT never resolves, the EnemyUnit will do nothing at all
                for (int checkingRadius = 1; checkingRadius < unitStats.MOVE; checkingRadius++) {

                    // the newPotentialEndpoint will NOT include the original maskedPathTo.End area, because we start at 1
                    foreach (GridPosition newPotentialEndpoint in maskedPathTo.End.Radiate(checkingRadius, min: checkingRadius-1)) {
                        if (moveRange.ValidMoveTo(newPotentialEndpoint)) {
                            pathTo = moveRange.BFS(gridPosition, newPotentialEndpoint);

                            if (pathTo != null) {
                                // now, use that path we just did all the hoopla on
                                // after reaching your destination, officially move via unitMap
                                selectedDmgPkg = candidateDmgPkg;

                                // since they are in the best order possible, break when you get your first candidate
                                return;
                            }
                        }
                    }
                }
                // if you made it here, check the next DamagePackage.
            }
        }

        // if you're not in a BrainPod, you're using your own AttackRange as your detection range
        // which means you won't do anything, and you wouldn't have made it here in the code
    }

    public IEnumerator ExecuteDamagePackage(EnemyBrain.DamagePackage selectedDmgPkg, Path<GridPosition> pathTo) {
        //
        // execute movement portion
        StartCoroutine( spriteAnimator.SmoothMovementPath<GridPosition>(pathTo, battleMap) );

        unitMap.ReservePosition(this, pathTo.End);
        _reservedGridPosition = pathTo.End;  // save for ContextualNoInteract to move via unitMap
        moveAvailable = false;
        ChangeState(EnemyUnitFSM.Moving);

        // WAIT FOR MOVEMENT TO COMPLETE
        yield return new WaitUntil(() => state == EnemyUnitFSM.Idle);
        // WAIT FOR MOVEMENT TO COMPLETE

        if (selectedDmgPkg.executableThisTurn) {
            Engagement engagement = Engagement.Create(this, selectedDmgPkg.target);
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
        }


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
