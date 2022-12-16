using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Mutations/CritBoostMut")]
public class CritBoostMut : Mutation
{
    public override void OnAcquire(Unit thisUnit) {
        thisUnit.OnAttack += Boost;
	}
    public override void OnRemove(Unit thisUnit) {
        thisUnit.OnAttack -= Boost;
	}

    private void Boost(Unit thisUnit, ref MutableAttack mutAtt, Unit target) {
        mutAtt.critRate += 25;
        mutAtt.AddMutator(this);
    }
}