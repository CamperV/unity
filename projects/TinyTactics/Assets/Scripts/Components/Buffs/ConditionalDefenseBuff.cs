using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ConditionalDefenseBuff : ConditionalBuff
{
    public override string displayName => $"+{modifierValue} Defense ({provider})";
    public override string affectedStat => "DEFENSE";

    public override void OnAcquire() {
        boundUnit.unitMap.NewBoardStateEvent += CheckCondition;
        boundUnit.OnAttack += DisplayBuffAttack;
        boundUnit.OnDefend += DisplayBuffDefense;
        
        boundUnit.unitStats.UpdateDefense(boundUnit.unitStats.DEFENSE + modifierValue);
    }

    public override void OnExpire() {
        boundUnit.unitMap.NewBoardStateEvent -= CheckCondition;
        boundUnit.OnAttack -= DisplayBuffAttack;
        boundUnit.OnDefend -= DisplayBuffDefense;

        boundUnit.unitStats.UpdateDefense(boundUnit.unitStats.DEFENSE - modifierValue);
    }

    private void DisplayBuffAttack(ref MutableAttack mutAtt, Unit target) {
        if (ConditionValid()) mutAtt.AddMutator(this);
    }

    private void DisplayBuffDefense(ref MutableDefense mutDef, Unit target) {
        if (ConditionValid()) mutDef.AddMutator(this);
    }
}