using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Extensions;

public class MutationSystem : MonoBehaviour
{
    private Unit boundUnit;

    // for binding UI, etc
    public delegate void MutationEvent(Mutation mutation);
    public event MutationEvent AddMutationEvent;
    public event MutationEvent RemoveMutationEvent;
    public event MutationEvent MutationTriggeredEvent;

    // assingable in Inspector for prefabs, but can be modified
    public List<Mutation> mutations;    
    
    void Awake() {
        boundUnit = GetComponent<Unit>();
    }

    // avoid using Start() because of potential race conditions
    // Initialize() is called by the composed Unit gameObject
    public void Initialize() {
        foreach (Mutation mut in mutations) {
            mut.OnAcquire(boundUnit);
        }
    }

    public void AddMutation(Mutation mutation) {
        mutations.Add(mutation);
        mutation.OnAcquire(boundUnit);

        AddMutationEvent?.Invoke(mutation);
    }

    public void RemoveMutation(Mutation mutation) {
        if (!mutations.Contains(mutation)) return;
        
        mutations.Remove(mutation);
        mutation.OnRemove(boundUnit);

        RemoveMutationEvent?.Invoke(mutation);
    }

    public void MutationTriggered(Mutation mutation) {
        MutationTriggeredEvent?.Invoke(mutation);
    }
}
