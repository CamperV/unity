using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public struct Defense
{
    public int damageReduction;
    public int critAvoidRate;
    public int advantageRate; // REFLEX
    //
    public List<string> mutators;

    public Defense(MutableDefense mutDef) {
        damageReduction = mutDef.damageReduction;
        critAvoidRate = mutDef.critAvoidRate;
        advantageRate = mutDef.advantageRate;
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
    public int advantageRate; // REFLEX
    //
    public List<string> mutators;

    public MutableDefense(int dr, int critAvoid, int reflex) {
        damageReduction = dr;
        critAvoidRate = critAvoid;
        advantageRate = reflex;
        //
        mutators = new List<string>();
    }

    public void AddMutator(IMutatorComponent mc) {
        mutators.Add(mc.displayName);
    }

    public void AddBonusDamageReduction(int add) {
        damageReduction += add;
    }
}