using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUnitController : MonoBehaviour, IStateMachine<PlayerUnitController.ControllerFSM>
{
    // debug
    public Text debugStateLabel;

    [SerializeField] private List<PlayerUnit> _activeUnits;
    public List<PlayerUnit> activeUnits {
        get => _activeUnits.Where(en => en.gameObject.activeInHierarchy).ToList();
    }

    public enum ControllerFSM {
        Inactive,
        NoSelection,
        Selection
    }
    [SerializeField] public ControllerFSM state { get; set; } = ControllerFSM.Inactive;

    // for determining how long the mouse is held over certain units
    private float holdTimeElapsed;
    private Coroutine holdTimer;
    private PlayerUnit holdUnit;
    private Color holdUnitOG;

    private PlayerUnit _currentSelection;
    private PlayerUnit currentSelection {
        get => _currentSelection;
        set {
            _currentSelection = value;
            if (_currentSelection == null) {
                ChangeState(ControllerFSM.NoSelection);
            } else {
                ChangeState(ControllerFSM.Selection);
            }
        }
    }

    void Start() {
        // this accounts for all in-scene activeUnits, not instatiated prefabs
        foreach (PlayerUnit en in GetComponentsInChildren<PlayerUnit>()) {
            _activeUnits.Add(en);
        }

        EnterState(ControllerFSM.NoSelection);
    }

    void Update() {
        ContextualNoInteract();
    }

    public void ChangeState(ControllerFSM newState) {
        ExitState(state);
        EnterState(newState);
    }

    public void InitialState() {
        ExitState(state);
        EnterState(ControllerFSM.NoSelection);
    }

    public void ExitState(ControllerFSM exitingState) {
        switch (exitingState) {
            case ControllerFSM.Inactive:
            case ControllerFSM.NoSelection:
            case ControllerFSM.Selection:
                break;
        }
        state = ControllerFSM.Inactive;
    }

    public void EnterState(ControllerFSM enteringState) {
        state = enteringState;
    
        // debug
        debugStateLabel.text = $"PlayerUnitController: {state.ToString()}";

        switch (state) {
            case ControllerFSM.Inactive:
            case ControllerFSM.NoSelection:
            case ControllerFSM.Selection:
                break;
        }
    }

    public void TriggerPhase() {
        ChangeState(ControllerFSM.NoSelection);
    }

    // we refresh at the end of the phase,
    // because we want color when it isn't your turn,
    // and because it's possible the other team could add statuses that 
    // disable attackAvailable/moveAvailable etc
    public void EndPhase() {
        ChangeState(ControllerFSM.Inactive);
    }

    public void RefreshUnits() => activeUnits.ForEach(it => it.RefreshInfo());

    public void ContextualInteractAt(GridPosition gp) {
        switch (state) {
            /////////////////////////////////////////////////////
            // When the Controller is inactive, we do nothing. //
            /////////////////////////////////////////////////////
            case ControllerFSM.Inactive:
                Debug.Log($"Sorry, I'm inactive");
                break;

            /////////////////////////////////////////////////////////////////////////////////
            // When the player interacts with the grid while there is no active selection, //
            // we attempt to make a selection.                                             //
            /////////////////////////////////////////////////////////////////////////////////
            case ControllerFSM.NoSelection:
                currentSelection = MatchingUnitAt(gp);
                currentSelection?.ContextualInteractAt(gp);
                break;

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // There are two things that can happen here:                                                             //
            //      1) If you click on a different unit, de-select current and select the new                         //
            //      2) If you don't, currentSelection will polymorphically decide what it wants to do (via its state) //
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            case ControllerFSM.Selection:
                PlayerUnit? unit = MatchingUnitAt(gp);

                // swap to the new unit. This will rapidly drop currentSelection (via Cancel/ChangeState(Idle))
                // then REACQUIRE a currentSelection immediately afterwards
                if (unit != null && unit != currentSelection) {
                    currentSelection.Cancel();
                    currentSelection = unit;
                }

                currentSelection.ContextualInteractAt(gp);
                break;
        }
    }

    public void ContextualNoInteract() {
        switch (state) {
            case ControllerFSM.Inactive:
            case ControllerFSM.NoSelection:
            case ControllerFSM.Selection:
                break;
        }
    }

    public void ClearSelection() {
        currentSelection = null;
    }

    public PlayerUnit? MatchingUnitAt(GridPosition gp) {
        foreach (PlayerUnit en in activeUnits) {
            if (en.gridPosition == gp) return en;
        }
        return null;
    }

    public void CheckEndPhase() {
        // check every time a unit finishes a turn
        // why don't we do this only on contextualActions?
        // because of the "Attacking" state. We must wait until animations are over
        bool endPlayerPhase = true;
        foreach (PlayerUnit unit in activeUnits) {
            endPlayerPhase &= !unit.turnActive;
        }
        
        if (endPlayerPhase) _EndPlayerPhase();
    }

    public void ForceEndPlayerPhase() {
        foreach (PlayerUnit u in activeUnits) {
            u.Cancel();
            u.FinishTurnNoCheck();
        }
        _EndPlayerPhase();
    }

    private void _EndPlayerPhase() => GetComponentInParent<TurnManager>().playerPhase.TriggerEnd();

    // this gets called when the BattleMap has a tile that a MouseHold event has triggered over
    public void StartHoldTimer(GridPosition gp) {
        PlayerUnit? unit = MatchingUnitAt(gp);

        // if we find a unit at this place, start the HoldTimer
        if (unit != null) {
            holdUnit = unit;
            holdUnitOG = unit.spriteRenderer.color;
            holdTimeElapsed = 0f;
            holdTimer = StartCoroutine( _HoldTimer(unit, 1f) );
        }
    }

    public void EndHoldTimer(GridPosition gp) {
        if (holdTimer != null) StopCoroutine(holdTimer);
        if (holdUnit != null) holdUnit.spriteRenderer.color = holdUnitOG;
        holdTimeElapsed = 0f;
    }

    private IEnumerator _HoldTimer(PlayerUnit unit, float maxTime) {
        Color originalColor = unit.spriteRenderer.color;

        // count until you reach maxTime
        while (holdTimeElapsed < maxTime) {

            // if the GridPosition  where the MouseHold was initiated is the same, continually count up
            if (unit.battleMap.CurrentMouseGridPosition == unit.gridPosition) {
                holdTimeElapsed += Time.deltaTime;

                float percComplete = holdTimeElapsed / maxTime;
                unit.spriteRenderer.color = Color.Lerp(originalColor, Color.magenta, percComplete);
                yield return null;
        
            // break out here if the mouse has been moved out of the tile
            } else {
                unit.spriteRenderer.color = originalColor;
                Debug.Log($"No way Jose");
                yield break;
            }
        }

        // if you've made it here, you legally completed holding down the mouse in one area
        // otherwise you would have exited early
        unit.spriteRenderer.color = originalColor;
        Debug.Log($"You made it!!!!");
    }
}