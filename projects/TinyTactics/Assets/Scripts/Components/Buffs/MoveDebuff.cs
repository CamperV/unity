using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MoveDebuff : ValuedStatus
{
    public override string displayName => $"{modifierValue} Move ({provider})";
    public override string affectedStat => "MOVE";

    public override void OnAcquire() {
        boundUnit.OnFinishTurn += TickExpire;
        boundUnit.OnAttack += DisplayDebuffAttack;
        boundUnit.OnDefend += DisplayDebuffDefense;

        boundUnit.unitStats.UpdateMove(boundUnit.unitStats.MOVE + modifierValue);
    }

    public override void OnExpire() {
        boundUnit.OnFinishTurn -= TickExpire;
        boundUnit.OnAttack -= DisplayDebuffAttack;
        boundUnit.OnDefend -= DisplayDebuffDefense;

        boundUnit.unitStats.UpdateMove(boundUnit.unitStats.MOVE - modifierValue);
    }

    private void DisplayDebuffAttack(ref MutableAttack mutAtt, Unit target) {
        mutAtt.AddMutator(this);
    }

    private void DisplayDebuffDefense(ref MutableDefense mutDef, Unit target) {
        mutDef.AddMutator(this);
    }
}