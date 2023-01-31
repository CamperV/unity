using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UnitController : MonoBehaviour, IUnitController, IPhaseController
{
    public delegate void RegistrationState(Unit unit);
    public event RegistrationState RegisteredUnit;

    [SerializeField] protected List<Unit> _units;

    // IUnitController
    public void RegisterUnit(Unit unit) {
        _units.Add(unit);
        RegisteredUnit?.Invoke(unit);
    }

    // IUnitController
    public List<Unit> GetActiveUnits() {
        return _units.Where(en => en.gameObject.activeInHierarchy).ToList();
    }

    // IUnitController
    public void RefreshUnits() {
        GetActiveUnits().ForEach(it => it.RefreshInfo());
    }

    public List<T> GetActiveUnits<T>() where T : Unit {
        return _units.Where(en => en.gameObject.activeInHierarchy).Select(u => u as T).ToList();
    }

    public virtual void TriggerPhase() {
        // re-focus the camera on the centroid of your units
        // Vector3[] unitPositions = activeUnits.Select(u => u.transform.position).ToArray();
        // CameraManager.FocusActiveCameraOn( VectorUtils.Centroid(unitPositions) );
        GetActiveUnits().ForEach(it => it.StartTurn());
    }

    // we refresh at the end of the phase,
    // because we want color when it isn't your turn,
    // and because it's possible the other team could add statuses that 
    public virtual void EndPhase() {}
}