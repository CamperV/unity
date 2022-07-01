using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ReflexDebuff : ValuedStatus
{
    public override string displayName => $"{modifierValue} Reflex ({provider})";
    public override string affectedStat => "REF";

    public override void OnAcquire() {
        boundUnit.OnFinishTurn += TickExpire;
        boundUnit.OnAttack += DisplayDebuffAttack;
        boundUnit.OnDefend += DisplayDebuffDefense;

        boundUnit.unitStats.UpdateReflex(boundUnit.unitStats.REFLEX + modifierValue);
    }

    public override void OnExpire() {
        boundUnit.OnFinishTurn -= TickExpire;
        boundUnit.OnAttack -= DisplayDebuffAttack;
        boundUnit.OnDefend -= DisplayDebuffDefense;

        boundUnit.unitStats.UpdateReflex(boundUnit.unitStats.REFLEX - modifierValue);
    }

    private void DisplayDebuffAttack(Unit thisUnit, ref MutableAttack mutAtt, Unit target) {
        mutAtt.AddMutator(this);
    }

    private void DisplayDebuffDefense(Unit thisUnit, ref MutableDefense mutDef, Unit target) {
        mutDef.AddMutator(this);
    }
}