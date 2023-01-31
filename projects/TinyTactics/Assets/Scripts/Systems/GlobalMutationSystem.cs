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

    public void AddGlobalMutation(Mutation globalMut) {
        globalMutations.Add(globalMut);
    }

    public void RemoveGlobalMutation(Mutation globalMut) {
        globalMutations.Remove(globalMut);
    }

    // this does a full clear: revokes then re-distributes if applicable
    public void RedistributeGlobalMutations() {
        foreach (Mutation mutation in globalMutations) {
            Revoke(mutation);

            // if you're conditional, check it. Otherwise, just add it
            if (mutation is IConditionalMutation) {
                if ((mutation as IConditionalMutation).ConditionMet(unitController.GetActiveUnits()))
                    Distribute(mutation);
            
            // if you're unconditional
            } else {
                Distribute(mutation);
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
