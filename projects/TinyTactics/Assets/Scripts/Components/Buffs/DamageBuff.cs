using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class DamageBuff : Buff
{
    public override string displayName => $"+{buffValue} Damage ({provider})";

    public override void OnAcquire() {
        boundUnit.OnFinishTurn += TickExpire;
        boundUnit.OnAttack += BuffAttackDamage;
    }

    public override void OnExpire() {
        boundUnit.OnFinishTurn -= TickExpire;
        boundUnit.OnAttack -= BuffAttackDamage;
    }

    private void BuffAttackDamage(ref MutableAttack mutAtt, Unit target) {
        mutAtt.damage += buffValue;
        mutAtt.AddMutator(this);
    }
}