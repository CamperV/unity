using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class StrengthDebuff : Debuff
{

    public override string displayName => $"-{modifierValue} Strength ({provider})";

    public override void OnAcquire() {
        boundUnit.OnFinishTurn += TickExpire;
        boundUnit.OnAttack += DisplayDebuffAttack;
        boundUnit.OnDefend += DisplayDebuffDefense;

        boundUnit.unitStats.UpdateStrength(boundUnit.unitStats.STRENGTH - modifierValue);
    }

    public override void OnExpire() {
        boundUnit.OnFinishTurn -= TickExpire;
        boundUnit.OnAttack -= DisplayDebuffAttack;
        boundUnit.OnDefend -= DisplayDebuffDefense;

        boundUnit.unitStats.UpdateStrength(boundUnit.unitStats.STRENGTH + modifierValue);
    }

    private void DisplayDebuffAttack(ref MutableAttack mutAtt, Unit target) {
        mutAtt.AddMutator(this);
    }

    private void DisplayDebuffDefense(ref MutableDefense mutDef, Unit target) {
        mutDef.AddMutator(this);
    }
}