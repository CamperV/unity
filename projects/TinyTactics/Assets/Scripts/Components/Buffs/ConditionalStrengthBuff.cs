using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ConditionalStrengthBuff : ConditionalBuff
{
    public override string displayName => $"+{buffValue} Strength ({provider})";

    public override void OnAcquire() {
        boundUnit.unitMap.NewBoardStateEvent += CheckCondition;
        boundUnit.OnAttack += DisplayBuffAttack;
        boundUnit.OnDefend += DisplayBuffDefense;
        
        boundUnit.unitStats.UpdateStrength(boundUnit.unitStats.STRENGTH + buffValue);
    }

    public override void OnExpire() {
        boundUnit.unitMap.NewBoardStateEvent -= CheckCondition;
        boundUnit.OnAttack -= DisplayBuffAttack;
        boundUnit.OnDefend -= DisplayBuffDefense;

        boundUnit.unitStats.UpdateStrength(boundUnit.unitStats.STRENGTH - buffValue);
    }

    private void DisplayBuffAttack(ref MutableAttack mutAtt, Unit target) {
        if (ConditionValid()) mutAtt.AddMutator(this);
    }

    private void DisplayBuffDefense(ref MutableDefense mutDef, Unit target) {
        if (ConditionValid()) mutDef.AddMutator(this);
    }
}