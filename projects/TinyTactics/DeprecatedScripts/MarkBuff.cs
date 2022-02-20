using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MarkBuff : CoupledBuff
{
    public override string displayName => $"+{modifierValue} Hit ({provider})";

    public override void OnAcquire() {
        boundUnit.OnFinishTurn += ExpireImmediately;
        boundUnit.OnAttack += BuffHit;
    }

    public override void OnExpire() {
        boundUnit.OnFinishTurn -= ExpireImmediately;
        boundUnit.OnAttack -= BuffHit;
    }

    private void BuffHit(ref MutableAttack mutAtt, Unit target) {
        if (coupledTarget == target) {
            // mutAtt.hitRate = (int)Mathf.Clamp(mutAtt.hitRate + modifierValue, 0f, 100f);
            mutAtt.hitRate += modifierValue;
            mutAtt.AddMutator(this);
        }
    }
}