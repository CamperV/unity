using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Mutations/CritBoostMut")]
public class CritBoostMut : Mutation
{
    public override void OnAcquire(Unit thisUnit) {
        thisUnit.OnAttackGeneration += Boost;
	}
    public override void OnRemove(Unit thisUnit) {
        thisUnit.OnAttackGeneration -= Boost;
	}

    private void Boost(Unit thisUnit, ref MutableAttack mutAtt, Unit target) {
        mutAtt.critRate += 25;
        mutAtt.AddAttackMutator(this);
    }
}