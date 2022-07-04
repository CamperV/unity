using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUnitController : MonoBehaviour, IUnitPhaseController
{
    // publicly acccessible events
    public delegate void UnitSelection(PlayerUnit selection);
    public event UnitSelection NewPlayerUnitControllerSelection;
    public event UnitSelection ClearPlayerUnitControllerSelection;

    // this is done by certain unit actions, so that you don't switch units when trying to heal them
    public bool selectionLocked = false;

    [SerializeField] private List<PlayerUnit> _activeUnits;
    public List<PlayerUnit> activeUnits {
        get => _activeUnits.Where(en => en.gameObject.activeInHierarchy).ToList();
    }
    public List<PlayerUnit> disabledUnits => _activeUnits.Where(en => !en.gameObject.activeInHierarchy).ToList();

    private PlayerUnit currentSelection;
    private PlayerUnit mostRecentlySelectedUnit;

    private UnitMap unitMap;
    private EnemyUnitController enemyUnitController;

    void Awake() {
        Battle _topBattleRef = GetComponentInParent<Battle>();
        unitMap = _topBattleRef.GetComponentInChildren<UnitMap>();
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
    }

    void Update() {
        if (currentSelection != null && currentSelection.turnActive == false) {
            ClearSelection();
            Debug.Log($"Fired, {selectionLocked}");
        }   
    }

    public void Lock() => selectionLocked = true;
    public void Unlock() => selectionLocked = false;

    public void RegisterUnit(PlayerUnit unit) => _activeUnits.Add(unit);

    public void TriggerPhase() {
        // re-focus the camera on the centroid of your units
        // Vector3[] unitPositions = activeUnits.Select(u => u.transform.position).ToArray();
        // CameraManager.FocusActiveCameraOn( VectorUtils.Centroid(unitPositions) );
        activeUnits.ForEach(it => it.StartTurn());
    }

    // we refresh at the end of the phase,
    // because we want color when it isn't your turn,
    // and because it's possible the other team could add statuses that 
    public void EndPhase() {
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
        enemyUnitController.ClearPreview();
        
        // we keep a rotating list of PlayerUnits in an Enumerator
        // we also can index into this to "start" at a certain unit
        // we do this here with "currentSelection.
        // however, if the currentSelection is the "last" unit in this rotating Enumerator,
        // it will fail. We fix this by keeping a "fallback" unit, which is the "first" unit
        // "first" -> min(gridPosition.y, gridPosition.x)
        PlayerUnit _fallbackUnit = activeUnits.OrderBy(u => u.gridPosition.y).ThenBy(u => u.gridPosition.x).ToList()[0];
        PlayerUnit nextUnit = GetNextUnit(currentSelection) ?? _fallbackUnit;

        // swap to the new unit. This will rapidly drop currentSelection (via Cancel/ChangeState(Idle))
        // then REACQUIRE a currentSelection immediately afterwards
        if (nextUnit != currentSelection) {
            ClearSelection();
            SetCurrentSelection(nextUnit);
        }
        currentSelection?.OnInteract(nextUnit.gridPosition, false);
    }

    public void ContextualInteractAt(GridPosition gp, bool auxiliaryInteract) {
        Debug.Log($"selectionLock: {selectionLocked}");
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // There are two things that can happen here:                                                             //
        //      1) If you click on a different unit, de-select current and select the new                         //
        //      2) If you don't, currentSelection will polymorphically decide what it wants to do (via its state) //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        Unit unit = unitMap.UnitAt(gp);

        if (unit != null && unit != currentSelection && !selectionLocked) {
            ClearSelection();
            enemyUnitController.ClearPreview();

            if (unit is PlayerUnit) {
                SetCurrentSelection((unit as PlayerUnit));

            } else if (unit is EnemyUnit) {
                enemyUnitController.Preview((unit as EnemyUnit));
            }
        }

        currentSelection?.OnInteract(gp, auxiliaryInteract);
    }

    public void SetCurrentSelection(PlayerUnit selection) {
        currentSelection = selection;
        CachedUnitEnumerator = GenerateCachedUnitEnumerator(mostRecentlySelectedUnit);

        if (selection == null) {
            //

        } else {
            mostRecentlySelectedUnit = selection;
            ClearPlayerUnitControllerSelection?.Invoke(mostRecentlySelectedUnit);
        }

        NewPlayerUnitControllerSelection?.Invoke(selection);
    }

    public void ClearSelection() {
        if (currentSelection != null && currentSelection.turnActive) {
            currentSelection.RevertTurn();
        }
        SetCurrentSelection(null);
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
            if (u.turnActive) u.WaitNoCheck();
        }
        _EndPlayerPhase();
    }

    private void _EndPlayerPhase() {
        SetCurrentSelection(null);
        GetComponentInParent<TurnManager>().playerPhase.TriggerEnd();
    }
}