using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

[Serializable]
public struct Attack
{
    public int damage;
    public int hitRate;
    public int critRate;
    //
    public bool inMeleeRange;
    //
    public List<string> mutators;

    public Attack(MutableAttack mutAtt) {
        damage = mutAtt.damage;
        hitRate = mutAtt.hitRate;
        critRate = mutAtt.critRate;
        //
        inMeleeRange = mutAtt.inMeleeRange;
        //
        mutators = new List<string>(mutAtt.mutators);
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
    //
    public bool inMeleeRange;
    //
    public List<string> mutators;

    public MutableAttack(int dmg, int hit, int crit, bool withinOne) {
        damage = dmg;
        hitRate = hit;
        critRate = crit;
        //
        inMeleeRange = withinOne;
        //
        mutators = new List<string>();
    }

    public void AddMutator(IMutatorComponent mc) {
        mutators.Add(mc.displayName);
    }
}