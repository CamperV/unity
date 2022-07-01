using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ConditionalDexterityBuff : ConditionalBuff
{
    public override string displayName => $"+{modifierValue} Dexterity ({provider})";
    public override string affectedStat => "DEX";

    public override void OnAcquire() {
        boundUnit.unitMap.NewBoardStateEvent += CheckCondition;
        boundUnit.OnAttack += DisplayBuffAttack;
        boundUnit.OnDefend += DisplayBuffDefense;
        
        boundUnit.unitStats.UpdateDexterity(boundUnit.unitStats.DEXTERITY + modifierValue);
    }

    public override void OnExpire() {
        boundUnit.unitMap.NewBoardStateEvent -= CheckCondition;
        boundUnit.OnAttack -= DisplayBuffAttack;
        boundUnit.OnDefend -= DisplayBuffDefense;

        boundUnit.unitStats.UpdateDexterity(boundUnit.unitStats.DEXTERITY - modifierValue);
    }

    private void DisplayBuffAttack(Unit thisUnit, ref MutableAttack mutAtt, Unit target) {
        if (ConditionValid()) mutAtt.AddMutator(this);
    }

    private void DisplayBuffDefense(Unit thisUnit, ref MutableDefense mutDef, Unit target) {
        if (ConditionValid()) mutDef.AddMutator(this);
    }
}