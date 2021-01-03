using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Attack
{
    public int damage;
    public int hitRate;
    public int critRate;
    public string attackType;

    public Attack(int dmg, int hit, int crit, string type) {
        damage = dmg;
        hitRate = hit;
        critRate = crit;
        attackType = type;

        Debug.Log($"Attack - DMG {damage}, HIT {hitRate}%, CRIT {critRate}%");
    }

    public void Modify(Unit a, Unit b) {}
}