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

    // assingable in Inspector for prefabs, but can be modified
    public List<so_Status> statuses;    
    
    void Awake() {
        boundUnit = GetComponent<PlayerUnit>();
    }

    // avoid using Start() because of potential race conditions
    // Initialize() is called by the composed Unit gameObject
    public void Initialize() {
        foreach (so_Status status in statuses) {
            status.OnAcquire(boundUnit);
        }
    }

    public void AddStatus(so_Status status) {
        statuses.Add(status);
        status.OnAcquire(boundUnit);
    }

    public void RemoveStatus(so_Status status) {
        statuses.Remove(status);
        status.OnRemove(boundUnit);
    }

    public bool HasStatus(so_Status _status) {
        foreach (so_Status status in statuses) {
            if (status == _status) return true;
        }
        return false;
    }
}
