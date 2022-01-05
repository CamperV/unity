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

    public static float timeBetweenUnitActions = 1.0f; // seconds

    // debug
    public Text debugStateLabel;

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

        // debug
        debugStateLabel.text = $"EnemyUnitController: {state.ToString()}";

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
            case ControllerFSM.TakeActions:
                playerUnitController.ChangeState(PlayerUnitController.ControllerFSM.NoSelection);
                break;
        }
        state = ControllerFSM.Inactive;
    }

    public void TriggerPhase() {
        activeUnits.ForEach(it => it.StartTurn() );

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

        foreach (EnemyUnit unit in activeUnits.OrderBy(unit => unit.Initiative)) {
            // if you've been cancelled, say by the Battle ending/turnManager suspending
            if (state != ControllerFSM.TakeActions) yield break;

            // Brain: get optimal target
            // Brain: find optimal position to attack target
            // two tiers: can reach (in MoveRange), can't reach
            // TODO improved AI: if you'll probably die when attacking "optimal target in move range",
            // switch to getting optimal target instead, even if not in the move range

            // if moving without attacking, wait a short bit and execute the next unit's action
            // if you're going to attack, let the entire thing play out before moving on
            // try to batch these together!
            // ie, move all "non-attackers" first, then play the deferred attackers
            // aactually, do the attackers first, because a unit going down might affect how the next unit's turn works
            
            // wait until the unit says you can move on
            // generally this is until the unit's turn is over,
            // but if the unit is only moving (and not attacking), just execute the next unit's whole situation
            yield return new WaitUntil(() => !unit.spriteAnimator.isAnimating);
            yield return unit.TakeActionFlowChart();
            yield return new WaitForSeconds(timeBetweenUnitActions);
        }

        GetComponentInParent<TurnManager>().enemyPhase.TriggerEnd();
	}
}