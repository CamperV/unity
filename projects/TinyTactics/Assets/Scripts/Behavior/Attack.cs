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

    public Attack(MutableAttack mutAtt) {
        damage = mutAtt.damage;
        hitRate = mutAtt.hitRate;
        critRate = mutAtt.critRate;  
    }

    public string ToString() {
        return $"Attack: {damage}/{hitRate}/{critRate}";
    }
}

//
// This is a class because I would like to mutate it via a Unit's stats, etc
public class MutableAttack
{
    public int damage;
    public int hitRate;
    public int critRate;

    public MutableAttack(int dmg, int hit, int crit) {
        damage = dmg;
        hitRate = hit;
        critRate = crit;
    }
}