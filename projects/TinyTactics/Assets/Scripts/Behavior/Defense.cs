using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public struct Defense
{
    public int damageReduction;
    public int dodgeRate;
    public int critDodgeRate;

    public Defense(int dr, int avoid, int critAvoid) {
        damageReduction = dr;
        dodgeRate = avoid;
        critDodgeRate = critAvoid;
    }

    public Defense(MutableDefense mutDef) {
        damageReduction = mutDef.damageReduction;
        dodgeRate = mutDef.dodgeRate;
        critDodgeRate = mutDef.critDodgeRate;  
    }

    public string ToString() {
        return $"Defense: {damageReduction}/{dodgeRate}/{critDodgeRate}";
    }
}

//
// This is a class because I would like to mutate it via a Unit's stats, etc
public class MutableDefense
{
    public int damageReduction;
    public int dodgeRate;
    public int critDodgeRate;

    public MutableDefense(int dr, int avoid, int critAvoid) {
        damageReduction = dr;
        dodgeRate = avoid;
        critDodgeRate = critAvoid;
    }
}