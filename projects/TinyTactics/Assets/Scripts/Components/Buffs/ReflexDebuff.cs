using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ReflexDebuff : Buff
{

    public override string displayName => $"-{buffValue} Reflex ({provider})";

    public override void OnAcquire() {
        boundUnit.OnFinishTurn += TickExpire;
        boundUnit.OnAttack += DisplayDebuffAttack;
        boundUnit.OnDefend += DisplayDebuffDefense;

        boundUnit.unitStats.UpdateReflex(boundUnit.unitStats.REFLEX - buffValue);
    }

    public override void OnExpire() {
        boundUnit.OnFinishTurn -= TickExpire;
        boundUnit.OnAttack -= DisplayDebuffAttack;
        boundUnit.OnDefend -= DisplayDebuffDefense;

        boundUnit.unitStats.UpdateReflex(boundUnit.unitStats.REFLEX + buffValue);
    }

    private void DisplayDebuffAttack(ref MutableAttack mutAtt, Unit target) {
        mutAtt.AddMutator(this);
    }

    private void DisplayDebuffDefense(ref MutableDefense mutDef, Unit target) {
        mutDef.AddMutator(this);
    }
}