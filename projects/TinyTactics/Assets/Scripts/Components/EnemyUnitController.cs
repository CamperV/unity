using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUnitController : MonoBehaviour, IStateMachine<EnemyUnitController.ControllerFSM>, IUnitPhaseController
{
    public static float timeBetweenUnitActions = 1.0f; // seconds

    // debug
    public Text debugStateLabel;

    [SerializeField] private List<EnemyUnit> _activeUnits;
    public List<EnemyUnit> activeUnits {
        get => _activeUnits.Where(en => en.gameObject.activeInHierarchy).ToList();
    }

    public enum ControllerFSM {
        Inactive,
        NoPreview,
        Preview,
        TakeActions
    }
    [SerializeField] public ControllerFSM state { get; set; } = ControllerFSM.Inactive;

    private EnemyUnit _currentPreview;
    private EnemyUnit currentPreview {
        get => _currentPreview;
        set {
            _currentPreview = value;
            if (_currentPreview == null) {
                ChangeState(ControllerFSM.NoPreview);
            } else {
                ChangeState(ControllerFSM.Preview);
            }
        }
    }

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

        EnterState(ControllerFSM.NoPreview);
    }

    public void ChangeState(ControllerFSM newState) {
        ExitState(state);
        EnterState(newState);
    }

    public void InitialState() {
        ExitState(state);
        EnterState(ControllerFSM.NoPreview);
    }

    public void EnterState(ControllerFSM enteringState) {
        state = enteringState;

        // debug
        debugStateLabel.text = $"EnemyUnitController: {state.ToString()}";

        switch (state) {
            case ControllerFSM.Inactive:
            case ControllerFSM.NoPreview:
            case ControllerFSM.Preview:
                break;

            case ControllerFSM.TakeActions:
                // disable enemy unit controller for a time
                playerUnitController.ChangeState(PlayerUnitController.ControllerFSM.Inactive);
                StartCoroutine( TakeActionAll() );
                break;
        }
    }

    public void ExitState(ControllerFSM exitingState) {
        switch (exitingState) {
            case ControllerFSM.Inactive:
            case ControllerFSM.NoPreview:
            case ControllerFSM.Preview:
                break;
                
            case ControllerFSM.TakeActions:
                playerUnitController.ChangeState(PlayerUnitController.ControllerFSM.NoSelection);
                break;
        }
        state = ControllerFSM.Inactive;
    }

    public void TriggerPhase() {
        ChangeState(ControllerFSM.TakeActions);
    }

    // we refresh at the end of the phase,
    // because we want color when it isn't your turn,
    // and because it's possible the other team could add statuses that 
    // disable attackAvailable/moveAvailable etc
    public void EndPhase() {
        activeUnits.ForEach(it => it.RefreshInfo());
        ChangeState(ControllerFSM.NoPreview);
    }

    public void ContextualInteractAt(GridPosition gp) {
        switch (state) {
            /////////////////////////////////////////////////////
            // When the Controller is inactive, we do nothing. //
            /////////////////////////////////////////////////////
            case ControllerFSM.Inactive:
                Debug.Log($"{this} is inactive, discarding input");
                break;

            /////////////////////////////////////////////////////////////////////////////////
            // When the player interacts with the grid while there is no active selection, //
            // we attempt to make a selection.                                             //
            /////////////////////////////////////////////////////////////////////////////////
            case ControllerFSM.NoPreview:
                currentPreview = MatchingUnitAt(gp);
                currentPreview?.ContextualInteractAt(gp);
                break;

            case ControllerFSM.Preview:
                EnemyUnit? unit = MatchingUnitAt(gp);

                // swap to the new unit. This will rapidly drop currentPreview (via Cancel/ChangeState(Idle))
                // then REACQUIRE a currentPreview immediately afterwards
                if (unit != null && unit != currentPreview) {
                    currentPreview.Cancel();
                    currentPreview = unit;
                }

                currentPreview.ContextualInteractAt(gp);
                break;

            case ControllerFSM.TakeActions:
                Debug.Log($"{this} is taking actions, discarding input");
                break;
        }
    }

    public void ClearPreview() {
        currentPreview = null;
    }

    private EnemyUnit? MatchingUnitAt(GridPosition gp) {
        foreach (EnemyUnit en in activeUnits) {
            if (en.gridPosition == gp) return en;
        }
        return null;
    }

	private IEnumerator TakeActionAll() {

        foreach (EnemyUnit unit in activeUnits.OrderBy(unit => unit.Initiative)) {
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
            Debug.Log($"Unit {unit} is taking actions");
            yield return unit.TakeActionFlowChart();
            Debug.Log($"Successfully got through {unit} Coroutine execution");
            yield return new WaitForSeconds(timeBetweenUnitActions);
        }

        GetComponentInParent<TurnManager>().enemyPhase.TriggerEnd();
	}
}