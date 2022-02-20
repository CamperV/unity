using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class OneTimeDamageBuff : ValuedStatus
{
    public override string displayName => $"+{modifierValue} Damage ({provider})";
    public override string affectedStat => "STR";

    public override void OnAcquire() {
        boundUnit.OnFinishTurn += ExpireImmediately;
        boundUnit.OnAttack += BuffAttackDamage;
    }

    public override void OnExpire() {
        boundUnit.OnFinishTurn -= ExpireImmediately;
        boundUnit.OnAttack -= BuffAttackDamage;
    }

    private void BuffAttackDamage(ref MutableAttack mutAtt, Unit target) {
        mutAtt.AddDamage(modifierValue);
        mutAtt.AddMutator(this);
    }
}