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
    }

    public void RemoveMutation(Mutation mutation) {
        mutations.Remove(mutation);
        mutation.OnRemove(boundUnit);
    }
}
