using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Extensions;

public class GlobalMutationSystem : MonoBehaviour
{
    [SerializeField] private UnitController unitController;
    [SerializeReference] public List<Mutation> globalMutations;

    public void Initialize() {
        CheckConditions();
    }

    public void AddGlobalMutation(Mutation globalMut) {
        globalMutations.Add(globalMut);
    }

    public void RemoveGlobalMutation(Mutation globalMut) {
        globalMutations.Remove(globalMut);
    }

    private void CheckConditions() {
        foreach (Mutation mutation in globalMutations) {
            if (true/*mutation.ConditionMet(unitController.GetActiveUnits())*/) {
                Distribute(mutation);
            } else { 
                Revoke(mutation);
            }
        }
    }

    private void Distribute(Mutation globalMutation) {
        foreach (Unit unit in unitController.GetActiveUnits()) {
            unit.mutationSystem.AddMutation(globalMutation);
        }
    }

    private void Revoke(Mutation globalMutation) {
        foreach (Unit unit in unitController.GetActiveUnits()) {
            unit.mutationSystem.RemoveMutation(globalMutation);
        }
    }
}
