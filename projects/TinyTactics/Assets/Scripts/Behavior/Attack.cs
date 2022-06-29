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
    public int dexterity;
    //
    public bool inMeleeRange;
    // public bool isCounterAttack;
    //
    public List<string> mutators;

    public Attack(MutableAttack mutAtt) {
        minDamage = mutAtt.minDamage;
        maxDamage = mutAtt.maxDamage;
        critRate = mutAtt.critRate;
        dexterity = mutAtt.dexterity;
        //
        inMeleeRange = mutAtt.inMeleeRange;
        // isCounterAttack = mutAtt.isCounterAttack;
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
    public int dexterity;
    //
    public bool inMeleeRange;
    // public bool isCounterAttack;
    //
    public List<string> mutators;

    public MutableAttack(int minDmg, int maxDmg, int crit, int dex, bool withinOne) {
        minDamage = minDmg;
        maxDamage = maxDmg;
        critRate = crit;
        dexterity = dex;
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