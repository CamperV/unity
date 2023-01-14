using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Mutations/ComboMut")]
public class ComboMut : Mutation
{
    public override void OnAcquire(Unit thisUnit) {
        // add thisUnit to the Combo Invocation list
        // ComboSystem.inst.
	}
    public override void OnRemove(Unit thisUnit) {
        // thisUnit.OnAttackGeneration -= Boost;
	}
}