using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ConditionalStrengthBuff : ConditionalBuff
{
    public override string displayName => $"+{modifierValue} Strength ({provider})";
    public override string affectedStat => "STR";

    public override void OnAcquire() {
        boundUnit.unitMap.NewBoardStateEvent += CheckCondition;
        boundUnit.OnAttack += DisplayBuffAttack;
        boundUnit.OnDefend += DisplayBuffDefense;
        
        boundUnit.unitStats.UpdateStrength(boundUnit.unitStats.STRENGTH + modifierValue);
    }

    public override void OnExpire() {
        boundUnit.unitMap.NewBoardStateEvent -= CheckCondition;
        boundUnit.OnAttack -= DisplayBuffAttack;
        boundUnit.OnDefend -= DisplayBuffDefense;

        boundUnit.unitStats.UpdateStrength(boundUnit.unitStats.STRENGTH - modifierValue);
    }

    private void DisplayBuffAttack(Unit thisUnit, ref MutableAttack mutAtt, Unit target) {
        if (ConditionValid()) mutAtt.AddMutator(this);
    }

    private void DisplayBuffDefense(Unit thisUnit, ref MutableDefense mutDef, Unit target) {
        if (ConditionValid()) mutDef.AddMutator(this);
    }
}