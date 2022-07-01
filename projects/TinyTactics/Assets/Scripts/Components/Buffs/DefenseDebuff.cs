using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class DefenseDebuff : ValuedStatus
{
    public override string displayName => $"{modifierValue} Defense ({provider})";
    public override string affectedStat => "DEF";

    public override void OnAcquire() {
        boundUnit.OnFinishTurn += TickExpire;
        boundUnit.OnAttack += DisplayDebuffAttack;
        boundUnit.OnDefend += DisplayDebuffDefense;

        boundUnit.unitStats.UpdateDefense(boundUnit.unitStats.DEFENSE + modifierValue);
    }

    public override void OnExpire() {
        boundUnit.OnFinishTurn -= TickExpire;
        boundUnit.OnAttack -= DisplayDebuffAttack;
        boundUnit.OnDefend -= DisplayDebuffDefense;

        boundUnit.unitStats.UpdateDefense(boundUnit.unitStats.DEFENSE - modifierValue);
    }

    private void DisplayDebuffAttack(Unit thisUnit, ref MutableAttack mutAtt, Unit target) {
        mutAtt.AddMutator(this);
    }

    private void DisplayDebuffDefense(ref MutableDefense mutDef, Unit target) {
        mutDef.AddMutator(this);
    }
}