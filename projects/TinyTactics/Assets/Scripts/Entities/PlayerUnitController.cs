using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUnitController : UnitController, IUnitController
{
    void Start() {
        // this accounts for all in-scene activeUnits, not instatiated prefabs
        // useful for Demo content
        foreach (PlayerUnit playerUnit in GetComponentsInChildren<PlayerUnit>()) {
            if (Campaign.active == null) {
                RegisterUnit((playerUnit as Unit));
            } else {
                Destroy(playerUnit.gameObject);
            }
        }

        // if you're not in an active Campaign, ie Demo, destroy all spawnMarkers
        if (Campaign.active == null) {
            foreach (SpawnMarker sm in GetComponentsInChildren<SpawnMarker>()) {
                Destroy(sm.gameObject);
            }
        }
    }

    public void CheckEndPhase() {
        // check every time a unit finishes a turn
        // why don't we do this only on contextualActions?
        // because of the "Attacking" state. We must wait until animations are over
        bool endPlayerPhase = true;
        foreach (Unit unit in GetActiveUnits()) {
            endPlayerPhase &= !unit.turnActive;
        }
        
        if (endPlayerPhase) _EndPlayerPhase();
    }

    public void ForceEndPlayerPhase() {
        foreach (Unit u in GetActiveUnits()) {
            if (u.turnActive) (u as PlayerUnit).WaitNoCheck();
        }
        _EndPlayerPhase();
    }

    private void _EndPlayerPhase() {
        GetComponentInParent<TurnManager>().playerPhase.TriggerEnd();
    }
}