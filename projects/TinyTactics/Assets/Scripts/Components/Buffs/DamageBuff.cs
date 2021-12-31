using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class DamageBuff : Buff
{
    public int expireTimer = 1;
    public int buffDamage = 1;

    public override string displayName => $"+{buffDamage} Damage ({provider})";

    public override void OnAcquire() {
        boundUnit.OnFinishTurn += TickExpire;
        boundUnit.OnAttack += BuffAttackDamage;
    }

    public override void OnExpire() {
        boundUnit.OnFinishTurn -= TickExpire;
        boundUnit.OnAttack -= BuffAttackDamage;
    }

    public void AddDamage(int dmg) {
        buffDamage += dmg;
    }

    public void TakeBestTimer(int timer) {
        expireTimer = Mathf.Max(expireTimer, timer);
    }

    private void TickExpire() {
        expireTimer--;

        if (expireTimer <= 0) {
            UIManager.inst.combatLog.AddEntry($"{boundUnit.logTag}@{boundUnit.name}'s <color=blue><b>{displayName}</b></color> expired.");
            Destroy(this);
        }
    }

    private void BuffAttackDamage(ref MutableAttack mutAtt, Unit target) {
        mutAtt.damage += buffDamage;
        mutAtt.AddMutator(this);
    }
}