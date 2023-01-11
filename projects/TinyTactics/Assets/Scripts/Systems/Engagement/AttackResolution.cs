using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

[Serializable]
public struct AttackResolution
{
    public int damageDealt;
    public int poiseDamageDealt;
    public bool isCrit;
    //
    public List<MutatorDisplayData> mutators;

    public AttackResolution(MutableAttackResolution mutAtt) {
        damageDealt = mutAtt.damageDealt;
        poiseDamageDealt = mutAtt.poiseDamageDealt;
        isCrit = mutAtt.isCrit;

        mutators = new List<MutatorDisplayData>(mutAtt.mutators);
    }
}

//
// This is a class because I would like to mutate it via a Unit's stats, etc
public class MutableAttackResolution
{
    public int damageDealt;
    public int poiseDamageDealt;
    public bool isCrit;
    //
    public List<MutatorDisplayData> mutators;

    public MutableAttackResolution(int dd, int pdd, bool c) {
        damageDealt = dd;
        poiseDamageDealt = pdd;
        isCrit = c;
        mutators = new List<MutatorDisplayData>();
    }

    public void AddMutator(IMutatorComponent mc) {
        mutators.Add(mc.mutatorDisplayData);
    } 
}