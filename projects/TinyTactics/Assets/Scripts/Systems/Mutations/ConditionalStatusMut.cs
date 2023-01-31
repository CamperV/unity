using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Mutations/ConditionalStatusMut")]
public class ConditionalStatusMut : StatusMut, IConditionalMutation
{
    public bool ConditionMet(List<Unit> units) {
        int purpleCount = 0;
        foreach (Unit unit in units) {
            foreach (MutationArchetype mutArchetype in unit.mutArchetypes) {
                if (mutArchetype.name == "Red") purpleCount++;
            }
        }
        return purpleCount > 0;
    }
}