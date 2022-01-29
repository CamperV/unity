using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class OneTimeDefenseBuff : ValuedStatus
{
    public override string displayName => $"+{modifierValue} Defense ({provider})";
    public override string affectedStat => "DEF";

    public override void OnAcquire() {
        boundUnit.OnStartTurn += ExpireImmediately;
        boundUnit.OnAttack += DisplayBuffAttack;
        boundUnit.OnDefend += DisplayBuffDefense;

        boundUnit.unitStats.UpdateDefense(boundUnit.unitStats.DEFENSE + modifierValue);
    }

    public override void OnExpire() {
        boundUnit.OnStartTurn -= ExpireImmediately;
        boundUnit.OnAttack -= DisplayBuffAttack;
        boundUnit.OnDefend -= DisplayBuffDefense;

        boundUnit.unitStats.UpdateDefense(boundUnit.unitStats.DEFENSE - modifierValue);
    }

    private void DisplayBuffAttack(ref MutableAttack mutAtt, Unit target) {
        mutAtt.AddMutator(this);
    }

    private void DisplayBuffDefense(ref MutableDefense mutDef, Unit target) {
        mutDef.AddMutator(this);
    }
}