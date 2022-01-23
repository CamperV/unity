using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUnitController : MonoBehaviour, IStateMachine<EnemyUnitController.ControllerFSM>, IUnitPhaseController
{
    // publicly acccessible events
    public delegate void UnitSelection(Unit selection);
    public event UnitSelection NewEnemyUnitControllerSelection;

    public static float timeBetweenUnitActions = 0.75f; // seconds

    [SerializeField] private List<EnemyUnit> _activeUnits;
    public List<EnemyUnit> activeUnits {
        get => _activeUnits.Where(en => en.gameObject.activeInHierarchy).ToList();
    }
    public List<EnemyUnit> disabledUnits => _activeUnits.Where(en => !en.gameObject.activeInHierarchy).ToList();

    public enum ControllerFSM {
        Inactive,
        NoPreview,
        Preview,
        TakeActions
    }
    [SerializeField] public ControllerFSM state { get; set; } = ControllerFSM.Inactive;

    private EnemyUnit currentPreview;
    private PlayerUnitController playerUnitController;

    void Awake() {
        Battle _topBattleRef = GetComponentInParent<Battle>();
        playerUnitController = _topBattleRef.GetComponentInChildren<PlayerUnitController>();
    }

    void Start() {
        // this accounts for all in-scene activeUnits, not instatiated prefabs
        foreach (EnemyUnit en in GetComponentsInChildren<EnemyUnit>()) {
            _activeUnits.Add(en);
        }

        InitialState();
    }

    public void ChangeState(ControllerFSM newState) {
        ExitState(state);
        EnterState(newState);
    }

    public void InitialState() {
        ExitState(state);
        EnterState(ControllerFSM.Inactive);
    }

    public void EnterState(ControllerFSM enteringState) {
        state = enteringState;

        switch (state) {
            case ControllerFSM.Inactive:
            case ControllerFSM.NoPreview:
                break;

            case ControllerFSM.Preview:
                // disable player unit controller for a time
                playerUnitController.ChangeState(PlayerUnitController.ControllerFSM.Inactive);
                break;

            case ControllerFSM.TakeActions:
                // disable player unit controller for a time
                playerUnitController.ChangeState(PlayerUnitController.ControllerFSM.Inactive);
                
                StartCoroutine( TakeActionAll() );
                break;
        }
    }

    public void ExitState(ControllerFSM exitingState) {
        switch (exitingState) {
            case ControllerFSM.Inactive:
            case ControllerFSM.NoPreview:
                break;

            case ControllerFSM.Preview:
                // disable player unit controller for a time
                playerUnitController.ChangeState(PlayerUnitController.ControllerFSM.NoSelection);
                break;

            case ControllerFSM.TakeActions:
                break;
        }
        state = ControllerFSM.Inactive;
    }

    public void TriggerPhase() {
        foreach (EnemyUnit unit in activeUnits) {
            unit.RefreshTargets();
            unit.StartTurn();
        }

        // also, update the threat ranges of the pods
        // we can't have this update every time a unit takes a turn
        BrainPod[] pods = GetComponentsInChildren<BrainPod>();
        foreach (BrainPod pod in pods) {
            pod.UpdateSharedDetectionRange();
        }

        // disable player unit controller for a time
        playerUnitController.ChangeState(PlayerUnitController.ControllerFSM.Inactive);
        ChangeState(ControllerFSM.TakeActions);
    }

    // we refresh at the end of the phase,
    // because we want color when it isn't your turn,
    // and because it's possible the other team could add statuses that 
    // disable attackAvailable/moveAvailable etc
    public void EndPhase() {
        ChangeState(ControllerFSM.NoPreview);
    }

    public void RefreshUnits() => activeUnits.ForEach(it => it.RefreshInfo());

    public void ContextualInteractAt(GridPosition gp) {
        switch (state) {
            /////////////////////////////////////////////////////
            // When the Controller is inactive, we do nothing. //
            /////////////////////////////////////////////////////
            case ControllerFSM.Inactive:
                break;

            /////////////////////////////////////////////////////////////////////////////////
            // When the player interacts with the grid while there is no active selection, //
            // we attempt to make a selection.                                             //
            /////////////////////////////////////////////////////////////////////////////////
            case ControllerFSM.NoPreview:
                SetCurrentPreview( MatchingUnitAt(gp) );
                currentPreview?.ContextualInteractAt(gp);
                break;

            case ControllerFSM.Preview:
                EnemyUnit? unit = MatchingUnitAt(gp);

                // swap to the new unit. This will rapidly drop currentPreview (via Cancel/ChangeState(Idle))
                // then REACQUIRE a currentPreview immediately afterwards
                if (unit != null && unit != currentPreview) {
                    ClearPreview();
                    SetCurrentPreview(unit);
                }

                currentPreview.ContextualInteractAt(gp);
                break;

            case ControllerFSM.TakeActions:
                break;
        }
    }

    public void SetCurrentPreview(EnemyUnit selection) {
        currentPreview = selection;

        if (selection == null) {
            ChangeState(ControllerFSM.NoPreview);
        } else {
            ChangeState(ControllerFSM.Preview);
        }

        NewEnemyUnitControllerSelection?.Invoke(selection);
    }

    public void ClearPreview() {
        if (state == ControllerFSM.Preview) {
            currentPreview.RevertTurn();
            SetCurrentPreview(null);
        }
    }

    private EnemyUnit? MatchingUnitAt(GridPosition gp) {
        foreach (EnemyUnit en in activeUnits) {
            if (en.gridPosition == gp) return en;
        }
        return null;
    }

	private IEnumerator TakeActionAll() {
        // first, try for any available Pods
        // they take their turns together, then take the stragglers
        BrainPod[] pods = GetComponentsInChildren<BrainPod>();
        foreach (BrainPod pod in pods.OrderBy(pod => pod.Initiative)) {

            foreach (EnemyUnit unit in pod.podmates.Where(it => it.turnActive).OrderBy(unit => unit.Initiative)) {
                // if you've been cancelled, say by the Battle ending/turnManager suspending
                if (state != ControllerFSM.TakeActions) yield break;

                yield return TakeAction(unit);
            }
        }

        // then, if the pods have gone, collect the stragglers (if their turn is still active)
        foreach (EnemyUnit unit in activeUnits.Where(it => it.turnActive).OrderBy(unit => unit.Initiative)) {
            // if you've been cancelled, say by the Battle ending/turnManager suspending
            if (state != ControllerFSM.TakeActions) yield break;

            yield return TakeAction(unit);
        }

        GetComponentInParent<TurnManager>().enemyPhase.TriggerEnd();
	}

    private IEnumerator TakeAction(EnemyUnit unit) {        
        // wait until the unit says you can move on
        // generally this is until the unit's turn is over,
        // but if the unit is only moving (and not attacking), just execute the next unit's whole situation
        yield return new WaitUntil(() => !unit.spriteAnimator.isAnimating);

        EnemyBrain.DamagePackage? selectedDmgPkg;
        Path<GridPosition>? pathTo;
        unit.SelectDamagePackage(out selectedDmgPkg, out pathTo);

        // if the unit wants to end early, let them
        // ie, unit can't actually execute the DamagePackage it wants to
        if (selectedDmgPkg == null) {
            unit.FinishTurn();

            // uncomment to focus the camera when they decide not to move            
            // NewEnemyUnitControllerSelection?.Invoke(unit);
            // yield return new WaitForSeconds(timeBetweenUnitActions/4f);

        // otherwise, if they want to take an action:
        // focus on it (camera)
        // execute the dang thing
        // and wait the normal amount between turns
        } else {
            NewEnemyUnitControllerSelection?.Invoke(unit);
            //
            yield return unit.ExecuteDamagePackage(selectedDmgPkg.Value, pathTo);
            yield return new WaitForSeconds(timeBetweenUnitActions);
        }
    }
}