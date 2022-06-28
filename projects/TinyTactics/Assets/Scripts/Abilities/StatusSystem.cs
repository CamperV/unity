using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Extensions;

public class StatusSystem : MonoBehaviour
{
    private PlayerUnit boundUnit;

    // for binding UI, etc
    public delegate void StatusEvent(so_Status status);
    public event StatusEvent AddStatusEvent;
    public event StatusEvent RemoveStatusEvent;

    // assingable in Inspector for prefabs, but can be modified
    [SerializeField] private List<so_Status> statuses;
    public IEnumerable<so_Status> Statuses => statuses;    

    private Dictionary<string, int> currentValues = new Dictionary<string, int>();
    private Dictionary<string, int> currentCountdowns = new Dictionary<string, int>();
    
    void Awake() {
        boundUnit = GetComponent<PlayerUnit>();
    }

    // avoid using Start() because of potential race conditions
    // Initialize() is called by the composed Unit gameObject
    public void Initialize() {
        boundUnit.OnStartTurn += TickExpireAll;
        boundUnit.OnStartTurn += CountdownAll;
        boundUnit.OnFinishTurn += ExpireImmediateAll;

        foreach (so_Status status in statuses) {
            status.OnAcquire(boundUnit);

            if (status.GetType() == typeof(so_ValueStatus)) {
                currentValues[status.name] = (status as so_ValueStatus).value;
            } else if (status.GetType() == typeof(so_CountdownStatus)) {
                currentCountdowns[status.name] = (status as so_CountdownStatus).value;
            }

            AddStatusEvent?.Invoke(status);
        }
    }

    public void AddStatus(so_Status status) {
        statuses.Add(status);
        status.OnAcquire(boundUnit);

        // i hate this type-checking nonsense, it should be better
        // but, on airplane again
        if (status.GetType() == typeof(so_ValueStatus)) {
            currentValues[status.name] = (status as so_ValueStatus).value;
        } else if (status.GetType() == typeof(so_CountdownStatus)) {
            currentCountdowns[status.name] = (status as so_CountdownStatus).value;
        }
        
        //
        AddStatusEvent?.Invoke(status);
    }

    public void RemoveStatus(so_Status status) {
        Debug.Log($"Removing {status} from {boundUnit}");
        statuses.Remove(status);
        status.OnExpire(boundUnit);

        if (currentValues.ContainsKey(status.name)) {
            currentValues.Remove(status.name);
        }
        if (currentCountdowns.ContainsKey(status.name)) {
            currentCountdowns.Remove(status.name);
        }
        
        //
        RemoveStatusEvent?.Invoke(status);
    }

    public bool HasStatus(so_Status _status) {
        // foreach (so_Status status in statuses) {
        //     if (status == _status) return true;
        // }
        // return false;
        return statuses.Contains(_status);
    }

    private void TickExpireAll(Unit _) {
        foreach (so_ValueStatus status in Statuses.OfType<so_ValueStatus>()) {
            if (status.expirationType == so_ValueStatus.ExpirationType.Tick) {
                int prevValue = currentValues[status.name];
                int newValue = (int)Mathf.MoveTowards(currentValues[status.name], 0f, 1f);
                
                // remove the previous effects, but re-apply the new value
                status.Apply(boundUnit, -prevValue);
                status.Apply(boundUnit, newValue);

                currentValues[status.name] = newValue;

                if (newValue == 0) RemoveStatus(status);
            }
        }
    }

    private void CountdownAll(Unit _) {
        foreach (so_CountdownStatus status in Statuses.OfType<so_CountdownStatus>()) {
            currentCountdowns[status.name] = (int)Mathf.MoveTowards(currentCountdowns[status.name], 0f, 1f);
            if (currentCountdowns[status.name] == 0) RemoveStatus(status);
        }
    }

    private void ExpireImmediateAll(Unit _) {
        foreach (so_ValueStatus status in Statuses.OfType<so_ValueStatus>()) {
            if (status.expirationType == so_ValueStatus.ExpirationType.Immediate) {
                status.Apply(boundUnit, -currentValues[status.name]);
                RemoveStatus(status);
            }
        }        
    }
}
