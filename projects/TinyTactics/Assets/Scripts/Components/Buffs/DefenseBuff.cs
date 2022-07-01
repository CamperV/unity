using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class DefenseBuff : ValuedStatus
{
    public override string displayName => $"+{modifierValue} Defense ({provider})";
    public override string affectedStat => "DEF";

    public override void OnAcquire() {
        boundUnit.OnFinishTurn += TickExpire;
        boundUnit.OnAttack += DisplayBuffAttack;
        boundUnit.OnDefend += DisplayBuffDefense;

        boundUnit.unitStats.UpdateDefense(boundUnit.unitStats.DEFENSE + modifierValue);
    }

    public override void OnExpire() {
        boundUnit.OnFinishTurn -= TickExpire;
        boundUnit.OnAttack -= DisplayBuffAttack;
        boundUnit.OnDefend -= DisplayBuffDefense;

        boundUnit.unitStats.UpdateDefense(boundUnit.unitStats.DEFENSE - modifierValue);
    }

    private void DisplayBuffAttack(Unit thisUnit, ref MutableAttack mutAtt, Unit target) {
        mutAtt.AddMutator(this);
    }

    private void DisplayBuffDefense(ref MutableDefense mutDef, Unit target) {
        mutDef.AddMutator(this);
    }
}