using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

[Serializable]
public struct Attack
{
    public int minDamage;
    public int maxDamage;
    public int critRate;
    //
    public bool inMeleeRange;
    //
    public List<string> mutators;

    public Attack(MutableAttack mutAtt) {
        minDamage = mutAtt.minDamage;
        maxDamage = mutAtt.maxDamage;
        critRate = mutAtt.critRate;
        //
        inMeleeRange = mutAtt.inMeleeRange;
        //
        mutators = new List<string>(mutAtt.mutators);
    }
}

//
// This is a class because I would like to mutate it via a Unit's stats, etc
public class MutableAttack
{
    public int minDamage;
    public int maxDamage;
    public int critRate;
    //
    public bool inMeleeRange;
    //
    public List<string> mutators;

    public MutableAttack(int minDmg, int maxDmg, int crit, bool withinOne) {
        minDamage = minDmg;
        maxDamage = maxDmg;
        critRate = crit;
        //
        inMeleeRange = withinOne;
        //
        mutators = new List<string>();
    }

    public void AddMutator(IMutatorComponent mc) {
        mutators.Add(mc.displayName);
    }

    public void AddDamage(int add) {
        minDamage += add;
        maxDamage += add;
    }
}

// [Serializable]
// public struct Attack
// {
//     public int damage;
//     public int hitRate;
//     public int critRate;
//     //
//     public bool inMeleeRange;
//     //
//     public List<string> mutators;

//     public Attack(MutableAttack mutAtt) {
//         damage = mutAtt.damage;
//         hitRate = mutAtt.hitRate;
//         critRate = mutAtt.critRate;
//         //
//         inMeleeRange = mutAtt.inMeleeRange;
//         //
//         mutators = new List<string>(mutAtt.mutators);
//     }

//     public string ToString() {
//         return $"Attack: {damage}/{hitRate}/{critRate}";
//     }
// }

// //
// // This is a class because I would like to mutate it via a Unit's stats, etc
// public class MutableAttack
// {
//     public int damage;
//     public int hitRate;
//     public int critRate;
//     //
//     public bool inMeleeRange;
//     //
//     public List<string> mutators;

//     public MutableAttack(int dmg, int hit, int crit, bool withinOne) {
//         damage = dmg;
//         hitRate = hit;
//         critRate = crit;
//         //
//         inMeleeRange = withinOne;
//         //
//         mutators = new List<string>();
//     }

//     public void AddMutator(IMutatorComponent mc) {
//         mutators.Add(mc.displayName);
//     }
// }