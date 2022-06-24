using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu (menuName = "Mutations/UnitCommandMutation")]
public class UnitCommandMutation : Mutation
{
    public UnitCommand unitCommand;

    public override void OnAcquire(Unit thisUnit) {
        if (thisUnit.GetType() == typeof(PlayerUnit)) {
            ((PlayerUnit)thisUnit).unitCommandSystem.AddCommand(unitCommand);
            Debug.Log($"Added {unitCommand} to {thisUnit}");
        }
    }

    public override void OnRemove(Unit thisUnit) {
        if (thisUnit.GetType() == typeof(PlayerUnit)) {
            (thisUnit as PlayerUnit).unitCommandSystem.RemoveCommand(unitCommand);
        }
    }
}