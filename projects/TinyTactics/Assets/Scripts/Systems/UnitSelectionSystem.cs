using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UnitSelectionSystem : MonoBehaviour
{
    // publicly acccessible events
    public UnityEvent<Unit> NewUnitSelection;

    [SerializeField] private UnitMap unitMap;
    [SerializeField] private PlayerUnitController playerUnitController;
    [SerializeField] private EnemyUnitController enemyUnitController;

    private Unit currentSelection;
    private PlayerUnit mostRecentlySelectedPlayerUnit;

    // this is done by certain unit actions, so that you don't switch units when trying to heal them
    private bool selectionLocked;
    public void Lock() => selectionLocked = true;
    public void Unlock() => selectionLocked = false;

    private List<Unit> _units;
    public List<PlayerUnit> ActivePlayerUnits => playerUnitController.GetActiveUnits<PlayerUnit>().Where(u => u.turnActive).ToList();   

    // this class exists to be an active GameObject in the scene,
    // to have children register themselves to various flags
    // this allows for nice, separable components
    public static UnitSelectionSystem inst = null; // enforces singleton behavior
	
    void Awake() {
 		if (inst == null) {
			inst = this;
		} else if (inst != this) {
			Destroy(gameObject);
		}
        
        _units = new List<Unit>();
    }

    void Start() {
        foreach (PlayerUnit playerUnit in playerUnitController.GetActiveUnits()) {
            _units.Add(playerUnit);
        }
        foreach (EnemyUnit enemyUnit in enemyUnitController.GetActiveUnits()) {
            _units.Add(enemyUnit);
        }
    }

    void OnEnable() => ClearSelection();
    void OnDisable() => ClearSelection();

    public void SelectNextPlayerUnit() {        
        // we keep a rotating list of ActivePlayerUnits in an Enumerator
        // we also can index into this to "start" at a certain unit
        // we do this here with "currentSelection.
        // however, if the currentSelection is the "last" unit in this rotating Enumerator,
        // it will fail. We fix this by keeping a "fallback" unit, which is the "first" unit
        // "first" -> min(gridPosition.y, gridPosition.x)
        PlayerUnit _fallbackUnit = ActivePlayerUnits.OrderBy(u => -u.gridPosition.y).ThenBy(u => u.gridPosition.x).ToList()[0];
        PlayerUnit nextUnit = GetNextPlayerUnit((currentSelection as PlayerUnit)) ?? _fallbackUnit;

        // swap to the new unit. This will rapidly drop currentSelection (via Cancel/ChangeState(Idle))
        // then REACQUIRE a currentSelection immediately afterwards
        if (nextUnit != currentSelection) {
            ClearSelection();
            SetCurrentSelection(nextUnit);
        }
        currentSelection?.OnInteract(nextUnit.gridPosition, false);
    }

    private PlayerUnit GetNextPlayerUnit(PlayerUnit currentUnit) {
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

    IEnumerator<PlayerUnit> CachedUnitEnumerator;
    private IEnumerator<PlayerUnit> GenerateCachedUnitEnumerator(PlayerUnit startingUnit) {
        bool seenStartingUnit = startingUnit == null;

        foreach (PlayerUnit unit in ActivePlayerUnits.OrderBy(u => -u.gridPosition.y)
                                                     .ThenBy(u => u.gridPosition.x)) {
            // trigger once only
            if (seenStartingUnit == false) {
                seenStartingUnit = unit == startingUnit;
            } else {
                yield return unit;
            }
        }
    }

    public void ClearSelection() {
        if (currentSelection != null) {
            if (currentSelection.turnActive) {
                currentSelection.RevertTurn();
            }
        }
        SetCurrentSelection(null);
    }

    private void SetCurrentSelection(Unit selection) {
        currentSelection = selection;
        if (currentSelection is PlayerUnit)
            CachedUnitEnumerator = GenerateCachedUnitEnumerator((currentSelection as PlayerUnit));
        NewUnitSelection?.Invoke(selection);
    }

    public void SelectAt(GridPosition gp) {
        if (gameObject.activeInHierarchy)
            _SelectAt(gp, false);
    }

    public void SelectAt_Aux(GridPosition gp) {
        if (gameObject.activeInHierarchy)
            _SelectAt(gp, true);
    }

    private void _SelectAt(GridPosition gp, bool auxiliaryInteract) {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // There are two things that can happen here:                                                             //
        //      1) If you click on a different unit, de-select current and select the new                         //
        //      2) If you don't, currentSelection will polymorphically decide what it wants to do (via its state) //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        Unit unit = unitMap.UnitAt(gp);
        if (unit != null && unit != currentSelection && !selectionLocked) {
            ClearSelection();
            SetCurrentSelection(unit);
        }

        currentSelection?.OnInteract(gp, auxiliaryInteract);
    }
}