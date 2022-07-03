using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUnitController : MonoBehaviour, IStateMachine<PlayerUnitController.ControllerFSM>
{
    // publicly acccessible events
    public delegate void UnitSelection(PlayerUnit selection);
    public event UnitSelection NewPlayerUnitControllerSelection;
    public event UnitSelection ClearPlayerUnitControllerSelection;

    // this is a flag that allows PlayerUnitController to be locked, ie can't ClearSelection()
    // this is done by certain unit actions, so that you don't switch units when trying to heal them
    public bool selectionLocked = false;
    private bool disabled = false;

    [SerializeField] private List<PlayerUnit> _activeUnits;
    public List<PlayerUnit> activeUnits {
        get => _activeUnits.Where(en => en.gameObject.activeInHierarchy).ToList();
    }
    public List<PlayerUnit> disabledUnits => _activeUnits.Where(en => !en.gameObject.activeInHierarchy).ToList();

    public enum ControllerFSM {
        NoSelection,
        Selection
    }
    [SerializeField] public ControllerFSM state { get; set; } = ControllerFSM.NoSelection;

    private PlayerUnit currentSelection;
    private PlayerUnit mostRecentlySelectedUnit;
    private EnemyUnitController enemyUnitController;

    void Awake() {
        Battle _topBattleRef = GetComponentInParent<Battle>();
        enemyUnitController = _topBattleRef.GetComponentInChildren<EnemyUnitController>();
    }

    void Start() {
        // this accounts for all in-scene activeUnits, not instatiated prefabs
        // useful for Demo content
        foreach (PlayerUnit en in GetComponentsInChildren<PlayerUnit>()) {
            if (Campaign.active == null) {
                _activeUnits.Add(en);
            } else {
                Destroy(en.gameObject);
            }
        }

        // if you're not in an active Campaign, ie Demo, destroy all spawnMarkers
        if (Campaign.active == null) {
            foreach (SpawnMarker sm in GetComponentsInChildren<SpawnMarker>()) {
                Destroy(sm.gameObject);
            }
        }

        InitialState();
    }

    void Update() {
        switch (state) {
            case ControllerFSM.NoSelection:
                break;

            ////////////////////////////////////////////////////////////////////////
            // as soon as your currentSelection finishes their turn, change state //
            ////////////////////////////////////////////////////////////////////////
            case ControllerFSM.Selection:
                if (currentSelection.turnActive == false) ClearSelection();
                break;
        }
    }

    public void Lock() => selectionLocked = true;
    public void Unlock() => selectionLocked = false;

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
            case ControllerFSM.NoSelection:
                break;

            case ControllerFSM.Selection:
                // re-enable EnemyUnitController
                // enemyUnitController.ChangeState(EnemyUnitController.ControllerFSM.NoPreview);
                break;
        }
    }

    public void EnterState(ControllerFSM enteringState) {
        state = enteringState;
    
        switch (state) {
            case ControllerFSM.NoSelection:
                break;

            case ControllerFSM.Selection:
                break;
        }
    }

    public void RegisterUnit(PlayerUnit unit) => _activeUnits.Add(unit);

    public void TriggerPhase() {
        // re-focus the camera on the centroid of your units
        // Vector3[] unitPositions = activeUnits.Select(u => u.transform.position).ToArray();
        // CameraManager.FocusActiveCameraOn( VectorUtils.Centroid(unitPositions) );

        disabled = false;
        ChangeState(ControllerFSM.NoSelection);

        activeUnits.ForEach(it => it.StartTurn());
    }

    // we refresh at the end of the phase,
    // because we want color when it isn't your turn,
    // and because it's possible the other team could add statuses that 
    public void EndPhase() {
        disabled = true;
        ChangeState(ControllerFSM.NoSelection);

        // if you end the phase, and you never selected anyone, choose the first just so the camera refocuses
        if (mostRecentlySelectedUnit == null) mostRecentlySelectedUnit = activeUnits[0];
    }

    public void RefreshUnits() => activeUnits.ForEach(it => it.RefreshInfo());

    IEnumerator<PlayerUnit> CachedUnitEnumerator;
    //
    private IEnumerator<PlayerUnit> GenerateCachedUnitEnumerator(PlayerUnit startingUnit) {
        bool seenStartingUnit = startingUnit == null;

        foreach (PlayerUnit unit in activeUnits.OrderBy(u => u.gridPosition.y).ThenBy(u => u.gridPosition.x)) {
            // trigger once only
            // if (firstSeen == null) firstSeen = unit;
            
            if (seenStartingUnit == false) {
                seenStartingUnit = unit == startingUnit;
            } else {
                yield return unit;
            }
        }
    }

    public PlayerUnit GetNextUnit(PlayerUnit currentUnit) {
        // overwrite previous enumerators
        if (CachedUnitEnumerator == null) CachedUnitEnumerator = GenerateCachedUnitEnumerator(currentUnit);

        // get next one
        bool success = CachedUnitEnumerator.MoveNext();
        if (!success) {
            CachedUnitEnumerator = GenerateCachedUnitEnumerator(currentUnit);
            CachedUnitEnumerator.MoveNext();
        }

        return CachedUnitEnumerator.Current;
    }

    public void SelectNextUnit() {
        // don't let us interrupt
        if (disabled) return;
        
        // enemyUnitController.ClearPreview();

        // we keep a rotating list of PlayerUnits in an Enumerator
        // we also can index into this to "start" at a certain unit
        // we do this here with "currentSelection.
        // however, if the currentSelection is the "last" unit in this rotating Enumerator,
        // it will fail. We fix this by keeping a "fallback" unit, which is the "first" unit
        // "first" -> min(gridPosition.y, gridPosition.x)
        PlayerUnit _fallbackUnit = activeUnits.OrderBy(u => u.gridPosition.y).ThenBy(u => u.gridPosition.x).ToList()[0];
        PlayerUnit nextUnit = GetNextUnit(currentSelection) ?? _fallbackUnit;

        switch (state) {
            case ControllerFSM.NoSelection:
                SetCurrentSelection(nextUnit);
                currentSelection?.OnInteract(nextUnit.gridPosition, false);
                break;

            case ControllerFSM.Selection:
                // swap to the new unit. This will rapidly drop currentSelection (via Cancel/ChangeState(Idle))
                // then REACQUIRE a currentSelection immediately afterwards
                if (nextUnit != currentSelection) {
                    ClearSelection();
                    SetCurrentSelection(nextUnit);
                }

                currentSelection.OnInteract(nextUnit.gridPosition, false);
                break;
        }
    }

    public void ContextualInteractAt(GridPosition gp, bool auxiliaryInteract) {
        switch (state) {
            /////////////////////////////////////////////////////////////////////////////////
            // When the player interacts with the grid while there is no active selection, //
            // we attempt to make a selection.                                             //
            /////////////////////////////////////////////////////////////////////////////////
            case ControllerFSM.NoSelection:
                SetCurrentSelection( MatchingUnitAt(gp) );
                currentSelection?.OnInteract(gp, auxiliaryInteract);
                break;

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // There are two things that can happen here:                                                             //
            //      1) If you click on a different unit, de-select current and select the new                         //
            //      2) If you don't, currentSelection will polymorphically decide what it wants to do (via its state) //
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            case ControllerFSM.Selection:
                if (!selectionLocked) {
                    PlayerUnit unit = MatchingUnitAt(gp);

                    // swap to the new unit. This will rapidly drop currentSelection (via Cancel/ChangeState(Idle))
                    // then REACQUIRE a currentSelection immediately afterwards
                    if (unit != null && unit != currentSelection) {
                        ClearSelection();
                        SetCurrentSelection(unit);
                    }
                }

                currentSelection.OnInteract(gp, auxiliaryInteract);
                break;
        }
    }

    public void SetCurrentSelection(PlayerUnit selection) {
        currentSelection = selection;
        CachedUnitEnumerator = GenerateCachedUnitEnumerator(currentSelection);

        if (selection == null) {
            ChangeState(ControllerFSM.NoSelection);
        } else {
            mostRecentlySelectedUnit = selection;
            ClearPlayerUnitControllerSelection?.Invoke(mostRecentlySelectedUnit);

            ChangeState(ControllerFSM.Selection);
        }

        NewPlayerUnitControllerSelection?.Invoke(selection);
    }

    public void ClearSelection() {
        if (state == ControllerFSM.Selection) {
            if (currentSelection.turnActive) currentSelection.RevertTurn();
            SetCurrentSelection(null);
        }
    }

    public PlayerUnit MatchingUnitAt(GridPosition gp) {
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
        if (!disabled) {
            foreach (PlayerUnit u in activeUnits) {
                if (u.turnActive) u.WaitNoCheck();
            }
            _EndPlayerPhase();
        }
    }

    private void _EndPlayerPhase() {
        SetCurrentSelection(null);
        GetComponentInParent<TurnManager>().playerPhase.TriggerEnd();
    }
}