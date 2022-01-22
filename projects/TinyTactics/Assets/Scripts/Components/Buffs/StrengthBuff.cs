using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class StrengthBuff : ValuedStatus
{
    public override string displayName => $"+{modifierValue} Strength ({provider})";
    public override string affectedStat => "STRENGTH";

    public override void OnAcquire() {
        boundUnit.OnFinishTurn += TickExpire;
        boundUnit.OnAttack += DisplayBuffAttack;
        boundUnit.OnDefend += DisplayBuffDefense;

        boundUnit.unitStats.UpdateStrength(boundUnit.unitStats.STRENGTH + modifierValue);
    }

    public override void OnExpire() {
        boundUnit.OnFinishTurn -= TickExpire;
        boundUnit.OnAttack -= DisplayBuffAttack;
        boundUnit.OnDefend -= DisplayBuffDefense;

        boundUnit.unitStats.UpdateStrength(boundUnit.unitStats.STRENGTH - modifierValue);
    }

    private void DisplayBuffAttack(ref MutableAttack mutAtt, Unit target) {
        mutAtt.AddMutator(this);
    }

    private void DisplayBuffDefense(ref MutableDefense mutDef, Unit target) {
        mutDef.AddMutator(this);
    }
}