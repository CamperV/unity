using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public struct Defense
{
    public int damageReduction;
    public int avoidRate;
    public int critAvoidRate;
    //
    public bool inMeleeRange;
    //
    public List<string> mutators;

    public Defense(MutableDefense mutDef) {
        damageReduction = mutDef.damageReduction;
        avoidRate = mutDef.avoidRate;
        critAvoidRate = mutDef.critAvoidRate;
        //
        inMeleeRange = mutDef.inMeleeRange;
        //
        mutators = new List<string>(mutDef.mutators); 
    }

    public string ToString() {
        return $"Defense: {damageReduction}/{avoidRate}/{critAvoidRate}";
    }
}

//
// This is a class because I would like to mutate it via a Unit's stats, etc
public class MutableDefense
{
    public int damageReduction;
    public int avoidRate;
    public int critAvoidRate;
    //
    public bool inMeleeRange;
    //
    public List<string> mutators;

    public MutableDefense(int dr, int avoid, int critAvoid, bool withinOne) {
        damageReduction = dr;
        avoidRate = avoid;
        critAvoidRate = critAvoid;
        //
        inMeleeRange = withinOne;
        //
        mutators = new List<string>();
    }

    public void AddMutator(IMutatorComponent mc) {
        mutators.Add(mc.displayName);
    }
}