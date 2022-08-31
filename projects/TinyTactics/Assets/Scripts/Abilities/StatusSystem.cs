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

    // keep state here
    private Dictionary<string, int> statusValues = new Dictionary<string, int>();
    private Dictionary<string, int> expireTimers = new Dictionary<string, int>();
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

                // string add = $"{status} [{statusProviderID}]";
                string add = $"{status}";

                if (status is IExpireStatus) {
                    add += $", expire:{expireTimers[statusProviderID]}";
                }
                if (status is IValueStatus) {
                    add += $", {statusValues[statusProviderID]}";
                }

                legibleStatuses.Add(add);
            }
        }

    }

    // avoid using Start() because of potential race conditions
    // Initialize() is called by the composed Unit gameObject
    public void Initialize() {
        boundUnit.OnStartTurn += StartTurn;
        boundUnit.OnFinishTurn += EndTurn;
        // boundUnit.OnStartTurn += CountdownExpireAll;
        // boundUnit.OnFinishTurn += ImmediateExpireAll;
    }

    public void AddStatus(so_Status status, string statusProviderID) {
        status.OnAcquire(boundUnit);

        // if you already have it and they can be combined:
        if (HasStatus(statusProviderID) && status.stackable) {
            if (status is IExpireStatus) expireTimers[statusProviderID] += (status as IExpireStatus).expireTimer;
            if (status is IValueStatus)  statusValues[statusProviderID] += (status as IValueStatus).value;
           
        // else if you don't have this Status already
        } else {
            statuses[statusProviderID] = status;

            if (status is IExpireStatus) expireTimers[statusProviderID] = (status as IExpireStatus).expireTimer;
            if (status is IValueStatus)  statusValues[statusProviderID] = (status as IValueStatus).value;
        }

        //
        AddStatusEvent?.Invoke(status);
    }

    public void RemoveStatus(string statusProviderID) {
        if (!HasStatus(statusProviderID)) return;

        so_Status status = statuses[statusProviderID];
        statuses.Remove(statusProviderID);
        status.OnExpire(boundUnit);

        if (expireTimers.ContainsKey(statusProviderID))
            expireTimers.Remove(statusProviderID);       
        //
        RemoveStatusEvent?.Invoke(status);
    }

    public bool HasStatus(string statusProviderID) {
        return statuses.ContainsKey(statusProviderID);
    }

    private void StartTurn(Unit _) {
        foreach (KeyValuePair<string, so_Status> kvp in new Dictionary<string, so_Status>(statuses)) {
            string statusProviderID = kvp.Key;
            so_Status status = kvp.Value;


            // if you're a TickStatus, you need to re-apply your value
            if (status is TickStatus) {
                int prevValue = statusValues[statusProviderID];
                int newValue = (int)Mathf.MoveTowards(statusValues[statusProviderID], 0f, 1f);
                
                // remove the previous effects, but re-apply the new value
                (status as TickStatus).Apply(boundUnit, -prevValue);
                (status as TickStatus).Apply(boundUnit, newValue);

                statusValues[statusProviderID] = newValue;
            }
        
            // all statuses with an expireTimer do this anyway
            if (status is IExpireStatus) {
                expireTimers[statusProviderID]--;
                if (expireTimers[statusProviderID] == 0) {
                    RemoveStatus(statusProviderID);
                }
            }
        }
    }

    private void EndTurn(Unit _) {
        foreach (KeyValuePair<string, so_Status> kvp in new Dictionary<string, so_Status>(statuses)) {
            string statusProviderID = kvp.Key;
            so_Status status = kvp.Value;

            if (status is IImmediateStatus) RemoveStatus(statusProviderID);
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
