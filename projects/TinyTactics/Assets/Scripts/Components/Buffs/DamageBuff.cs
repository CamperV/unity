using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class DamageBuff : Buff
{
    public int expireTimer = 1;
    public int buffDamage = 1;

    public override string displayName => $"+{buffDamage} Damage";

    public override void OnAcquire() {
        boundUnit.OnFinishTurn += TickExpire;
        boundUnit.OnAttack += BuffAttackDamage;
    }

    public override void OnExpire() {
        boundUnit.OnFinishTurn -= TickExpire;
        boundUnit.OnAttack -= BuffAttackDamage;

        UIManager.inst.combatLog.AddEntry($"{boundUnit.logTag}@{boundUnit.name}'s <color=blue><b>{displayName}</b></color> expired.");
        
        Destroy(this);
    }

    // what happens when a buff of the same type is added
    public override void Increment() {
        buffDamage++;
    }

    private void TickExpire() {
        expireTimer--;

        if (expireTimer <= 0) {
            OnExpire();
        }
    }

    private void BuffAttackDamage(ref MutableAttack mutAtt, Unit target) {
        mutAtt.damage += buffDamage;
        mutAtt.AddMutator(this);
    }
}