using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public struct Defense
{
    public int damageReduction;
    public int critAvoidRate;
    public int reflex; // REFLEX
    //
    public bool inMeleeRange;
    //
    public List<string> mutators;

    public Defense(MutableDefense mutDef) {
        damageReduction = mutDef.damageReduction;
        critAvoidRate = mutDef.critAvoidRate;
        reflex = mutDef.reflex;
        //
        inMeleeRange = mutDef.inMeleeRange;
        //
        mutators = new List<string>(mutDef.mutators); 
    }
}

//
// This is a class because I would like to mutate it via a Unit's stats, etc
public class MutableDefense
{
    public int damageReduction;
    public int critAvoidRate;
    public int reflex; // REFLEX
    //
    public bool inMeleeRange;
    //
    public List<string> mutators;

    public MutableDefense(int dr, int critAvoid, int _ref, bool withinOne) {
        damageReduction = dr;
        critAvoidRate = critAvoid;
        reflex = _ref;
        //
        inMeleeRange = withinOne;
        //
        mutators = new List<string>();
    }

    public void AddMutator(IMutatorComponent mc) {
        mutators.Add(mc.displayName);
    }

    public void AddDamageReduction(int add) {
        damageReduction += add;
    }
}