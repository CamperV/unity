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
        int boost = thisUnit.unitStats.FINESSE - target.unitStats.FINESSE;
        if (boost > 0) {
            mutAtt.critRate += thisUnit.unitStats.FINESSE * boost;   
        }
    }
}