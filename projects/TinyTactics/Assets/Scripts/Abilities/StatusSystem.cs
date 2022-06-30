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
    [SerializeField] private List<so_Status> statuses;
    public IEnumerable<so_Status> Statuses => statuses.ToList().AsEnumerable(); // for iterating + removing

    private Dictionary<string, int> expireValues = new Dictionary<string, int>();

    public List<string> active;
    public List<int> values;
    
    void Awake() {
        boundUnit = GetComponent<Unit>();
    }

    void Update() {
        active = expireValues.Keys.ToList();
        values = expireValues.Values.ToList();
    }

    // avoid using Start() because of potential race conditions
    // Initialize() is called by the composed Unit gameObject
    public void Initialize() {
        boundUnit.OnStartTurn += TickExpireAll;
        boundUnit.OnStartTurn += CountdownExpireAll;
        boundUnit.OnFinishTurn += ImmediateExpireAll;

        foreach (so_Status status in statuses) {
            AddStatus(status, addToList: false);
        }
    }

    public void AddStatus(so_Status status, bool addToList = true) {
        status.OnAcquire(boundUnit);

        // if you already have it and they can be combined:
        if (HasStatus(status) && status.stackable) {
            if (status is IValueStatus) {
                expireValues[status.name] += (status as IValueStatus).value;
            }
           

        // else if you don't have this Status already
        } else {
            if (addToList) statuses.Add(status);

            if (status is IValueStatus) {
                expireValues[status.name] = (status as IValueStatus).value;
            }
        }

        //
        AddStatusEvent?.Invoke(status);
    }

    public void RemoveStatus(so_Status status) {
        if (!HasStatus(status)) return;
        
        statuses.Remove(status);
        status.OnExpire(boundUnit);

        if (expireValues.ContainsKey(status.name))
            expireValues.Remove(status.name);       
        //
        RemoveStatusEvent?.Invoke(status);
    }

    public bool HasStatus(so_Status _status) {
        return statuses.Contains(_status);
    }

    // tick like normal, but re-apply the value to the unit
    private void TickExpireAll(Unit _) {
        foreach (TickValueStatus status in Statuses.OfType<TickValueStatus>()) {
            int prevValue = expireValues[status.name];
            int newValue = (int)Mathf.MoveTowards(expireValues[status.name], 0f, 1f);
            
            // remove the previous effects, but re-apply the new value
            status.Apply(boundUnit, -prevValue);
            status.Apply(boundUnit, newValue);

            expireValues[status.name] = newValue;

            if (newValue == 0) RemoveStatus(status);
        }
    }

    // tick like normal, but don't re-apply the value
    private void CountdownExpireAll(Unit _) {
        foreach (CountdownStatus status in Statuses.OfType<CountdownStatus>()) {
            Debug.Log($"{status} has a value {expireValues[status.name]}");
            expireValues[status.name] = (int)Mathf.MoveTowards(expireValues[status.name], 0f, 1f);
            Debug.Log($"{status} has a new value {expireValues[status.name]}");

            if (expireValues[status.name] == 0) RemoveStatus(status);
        }
    }

    // always remove the value
    private void ImmediateExpireAll(Unit _) {
        foreach (ImmediateValueStatus status in Statuses.OfType<ImmediateValueStatus>()) {
            status.Apply(boundUnit, -expireValues[status.name]);
            RemoveStatus(status);
        }        
    }
}
