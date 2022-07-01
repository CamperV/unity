using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ChainBuff : ValuedStatus
{
    public override string displayName => $"+{modifierValue} Damage ({provider})";
    public override string affectedStat => "STR";

    public override void OnAcquire() {
        boundUnit.OnAttack += DisplayBuffAttack;
        boundUnit.OnDefend += DisplayBuffDefense;

        boundUnit.unitStats.UpdateStrength(boundUnit.unitStats.STRENGTH + modifierValue);
    }

    public override void OnExpire() {
        boundUnit.OnAttack -= DisplayBuffAttack;
        boundUnit.OnDefend -= DisplayBuffDefense;

        boundUnit.unitStats.UpdateStrength(boundUnit.unitStats.STRENGTH - modifierValue);
    }

    private void DisplayBuffAttack(Unit thisUnit, ref MutableAttack mutAtt, Unit target) {
        mutAtt.AddMutator(this);
    }

    private void DisplayBuffDefense(Unit thisUnit, ref MutableDefense mutDef, Unit target) {
        mutDef.AddMutator(this);
    }
}