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
    private Unit boundUnit;

    // for binding UI, etc
    public delegate void StatusEvent(so_Status status);
    public event StatusEvent AddStatusEvent;
    public event StatusEvent RemoveStatusEvent;

    // assingable in Inspector for prefabs, but can be modified
    private Dictionary<string, so_Status> statuses = new Dictionary<string, so_Status>();
    private Dictionary<string, int> expireValues = new Dictionary<string, int>();
    //
    public IEnumerable<so_Status> Statuses => statuses.Values.ToList().AsEnumerable(); // for iterating + removing

    public bool enableLegibleStatuses;
    public List<string>  legibleStatuses;
    
    void Awake() {
        boundUnit = GetComponent<Unit>();

        legibleStatuses = new List<string>();
    }

    void Update() {
        if (enableLegibleStatuses) {
            legibleStatuses.Clear();

            foreach (KeyValuePair<string, so_Status> kvp in statuses) {
                string statusProviderID = kvp.Key;
                so_Status status = kvp.Value;

                string add = $"{status} [{statusProviderID}]";
                if (expireValues.ContainsKey(statusProviderID)) {
                    add += $"= {expireValues[statusProviderID]}";
                }
                legibleStatuses.Add(add);
            }
        }

    }

    // avoid using Start() because of potential race conditions
    // Initialize() is called by the composed Unit gameObject
    public void Initialize() {
        boundUnit.OnStartTurn += TickExpireAll;
        boundUnit.OnStartTurn += CountdownExpireAll;
        boundUnit.OnFinishTurn += ImmediateExpireAll;
    }

    public void AddStatus(so_Status status, string statusProviderID) {
        status.OnAcquire(boundUnit);

        // if you already have it and they can be combined:
        if (HasStatus(statusProviderID) && status.stackable) {
            if (status is IExpireStatus && status is IValueStatus) {
                expireValues[statusProviderID] += (status as IValueStatus).value;
            }
           

        // else if you don't have this Status already
        } else {
            statuses[statusProviderID] = status;

            if (status is IExpireStatus && status is IValueStatus) {
                expireValues[statusProviderID] = (status as IValueStatus).value;
            }
        }

        //
        AddStatusEvent?.Invoke(status);
    }

    public void RemoveStatus(string statusProviderID) {
        if (!HasStatus(statusProviderID)) return;

        so_Status status = statuses[statusProviderID];
        statuses.Remove(statusProviderID);
        status.OnExpire(boundUnit);

        if (expireValues.ContainsKey(statusProviderID))
            expireValues.Remove(statusProviderID);       
        //
        RemoveStatusEvent?.Invoke(status);
    }

    public bool HasStatus(string statusProviderID) {
        return statuses.ContainsKey(statusProviderID);
    }

    // tick like normal, but re-apply the value to the unit
    private void TickExpireAll(Unit _) {
        foreach (KeyValuePair<string, so_Status> kvp in new Dictionary<string, so_Status>(statuses)) {
            string statusProviderID = kvp.Key;
            so_Status status = kvp.Value;

            if (status is TickValueStatus) {
                int prevValue = expireValues[statusProviderID];
                int newValue = (int)Mathf.MoveTowards(expireValues[statusProviderID], 0f, 1f);
                
                // remove the previous effects, but re-apply the new value
                (status as TickValueStatus).Apply(boundUnit, -prevValue);
                (status as TickValueStatus).Apply(boundUnit, newValue);

                expireValues[statusProviderID] = newValue;

                if (newValue == 0) RemoveStatus(statusProviderID);
            }
        }
    }

    // tick like normal, but don't re-apply the value
    private void CountdownExpireAll(Unit _) {
        foreach (KeyValuePair<string, so_Status> kvp in new Dictionary<string, so_Status>(statuses)) {
            string statusProviderID = kvp.Key;
            so_Status status = kvp.Value;

            if (status is CountdownStatus) {
                expireValues[statusProviderID] = (int)Mathf.MoveTowards(expireValues[statusProviderID], 0f, 1f);
                if (expireValues[statusProviderID] == 0) RemoveStatus(statusProviderID);
            }
        }
    }

    // always remove the value
    private void ImmediateExpireAll(Unit _) {
        foreach (KeyValuePair<string, so_Status> kvp in new Dictionary<string, so_Status>(statuses)) {
            string statusProviderID = kvp.Key;
            so_Status status = kvp.Value;

            if (status is IImmediateStatus) {
                RemoveStatus(statusProviderID);
            }
        }        
    }

    public void RevertMovementStatuses() {
        foreach (KeyValuePair<string, so_Status> kvp in new Dictionary<string, so_Status>(statuses)) {
            string statusProviderID = kvp.Key;
            so_Status status = kvp.Value;

            if (status is IImmediateStatus) {
                if ((status as IImmediateStatus).revertWithMovement) {
                    RemoveStatus(statusProviderID);
                }
            }
        }        
    }
}
