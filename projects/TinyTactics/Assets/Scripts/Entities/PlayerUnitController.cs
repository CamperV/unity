using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUnitController : MonoBehaviour, IUnitPhaseController
{
    public delegate void RegistrationState(Unit unit);
    public event RegistrationState RegisteredUnit;

    [SerializeField] private List<PlayerUnit> _activeUnits;
    public List<PlayerUnit> activeUnits {
        get => _activeUnits.Where(en => en.gameObject.activeInHierarchy).ToList();
    }
    public List<PlayerUnit> disabledUnits => _activeUnits.Where(en => !en.gameObject.activeInHierarchy).ToList();

    void Start() {
        // this accounts for all in-scene activeUnits, not instatiated prefabs
        // useful for Demo content
        foreach (PlayerUnit playerUnit in GetComponentsInChildren<PlayerUnit>()) {
            if (Campaign.active == null) {
                RegisterUnit(playerUnit);
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

    public void RegisterUnit(PlayerUnit unit) {
        _activeUnits.Add(unit);
        RegisteredUnit?.Invoke( (unit as Unit) );
    }

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
        // do nothing?
    }

    public void RefreshUnits() => activeUnits.ForEach(it => it.RefreshInfo());

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
        GetComponentInParent<TurnManager>().playerPhase.TriggerEnd();
    }
}