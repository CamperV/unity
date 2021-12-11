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

    [SerializeField] public List<EnemyUnit> entities;

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

    void Start() {
        // this accounts for all in-scene Entities, not instatiated prefabs
        foreach (EnemyUnit en in GetComponentsInChildren<EnemyUnit>()) {
            entities.Add(en);
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

    public void ExitState(ControllerFSM exitingState) {
        switch (exitingState) {
            case ControllerFSM.Inactive:
            case ControllerFSM.NoPreview:
            case ControllerFSM.Preview:
            case ControllerFSM.TakeActions:
                break;
        }
        state = ControllerFSM.Inactive;
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
                StartCoroutine( TakeActionAll() );
                break;
        }
    }

    public void TriggerPhase() {
        ChangeState(ControllerFSM.TakeActions);
    }

    // we refresh at the end of the phase,
    // because we want color when it isn't your turn,
    // and because it's possible the other team could add statuses that 
    // disable attackAvailable/moveAvailable etc
    public void EndPhase() {
        entities.ForEach(it => it.RefreshInfo());
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
        foreach (EnemyUnit en in entities) {
            if (en.gridPosition == gp) return en;
        }
        return null;
    }

	private IEnumerator TakeActionAll() {
        // List<MovingGridObject> orderedRegistry = activeRegistry.OrderBy(it => (it as EnemyArmy).CalculateInitiative()).ToList();

        // public float CalculateInitiative() {
        //     int md = gridPosition.ManhattanDistance(GlobalPlayerState.army.gridPosition);

        //     float directionScore = 0.0f;
        //     switch (gridPosition - GlobalPlayerState.army.gridPosition) {
        //         case Vector3Int v when v.Equals(Vector3Int.up):
        //             directionScore = 0.0f;
        //             break;
        //         case Vector3Int v when v.Equals(Vector3Int.right):
        //             directionScore = 0.1f;
        //             break;
        //         case Vector3Int v when v.Equals(Vector3Int.down):
        //             directionScore = 0.2f;
        //             break;
        //         case Vector3Int v when v.Equals(Vector3Int.left):
        //             directionScore = 0.3f;
        //             break;
        //     }

        //     return (float)md + directionScore;
        // }
        foreach (EnemyUnit unit in entities.OrderBy(unit => unit.Initiative)) {
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
            unit.TakeActionFlowChart();
            while (unit.turnActive) yield return null;
            
            yield return new WaitForSeconds(timeBetweenUnitActions);
        }

        GetComponentInParent<TurnManager>().enemyPhase.TriggerEnd();
	}
}