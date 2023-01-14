using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Mutations/ComboMut")]
public class ComboMut : Mutation
{
    public override void OnAcquire(Unit thisUnit) {
        // add thisUnit to the Combo Invocation list
        EngagementSystem.inst.comboSystem.AddUnit(thisUnit);
	}
    public override void OnRemove(Unit thisUnit) {
        EngagementSystem.inst.comboSystem.RemoveUnit(thisUnit);
	}
}