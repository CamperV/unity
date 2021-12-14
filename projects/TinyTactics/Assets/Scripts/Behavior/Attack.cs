using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public struct Attack
{
    public int damage;
    public int hitRate;
    public int critRate;

    public Attack(int dmg, int hit, int crit) {
        damage = dmg;
        hitRate = hit;
        critRate = crit;
    }

    public string ToString() {
        return $"Attack: {damage}/{hitRate}/{critRate}";
    }
}