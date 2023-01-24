using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Extensions;
using UnityEngine.EventSystems;
using UnityEngine.Events;

interface IUnitInspector {
    void InspectUnit(Unit unit);
}

public class UnitInspectorSystem : MonoBehaviour
{
    // this class exists to be an active GameObject in the scene,
    // to have children register themselves to various flags
    // this allows for nice, separable components
    [HideInInspector] public Unit currentUnit;

	public UnityEvent<Unit> EnableInspectionEvent;
	public UnityEvent<Unit> DisableInspectionEvent;

    void OnDisable() {
        Disable(currentUnit ?? null);
    }

    public void InspectUnit(Unit unit) {
        if (unit != null) {
            unit.OnFinishTurn += AutoDisable;
            EnableInspectionEvent?.Invoke(unit);

        // this is for manually disabling
        // otherwise, the UnitInspectors are disabled while watching for unit.OnFinishTurn
        } else {
            Disable(currentUnit);
        }

        currentUnit = unit;
    }

    // this is triggered when a unit ends their turn
    // it should unregister itself from the unit
    private void AutoDisable(Unit finishedUnit) => Disable(finishedUnit);

    private void Disable(Unit finishedUnit) {
        if (finishedUnit != null)
            finishedUnit.OnFinishTurn -= AutoDisable;
        DisableInspectionEvent?.Invoke(finishedUnit);
    }
}