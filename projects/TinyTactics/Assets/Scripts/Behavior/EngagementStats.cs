using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

[Serializable]
public struct EngagementStats {
    public int minDamage;
    public int maxDamage;
    public int critRate;
    public int advantageRate;   // this is an integer that represents how many times you can re-roll and take the higher number
    public List<string> mutators;

    public EngagementStats(int minDmg, int maxDmg, int cr, int adv) {
        minDamage = minDmg;
        maxDamage = maxDmg;
        critRate = cr;
        advantageRate = adv;
        //
        mutators = new List<string>();
    }

    public EngagementStats(MutableEngagementStats mutES) {
        minDamage = mutES.minDamage;
        maxDamage = mutES.maxDamage;
        critRate = mutES.critRate;
        advantageRate = mutES.advantageRate;
        //
        mutators = new List<string>(mutES.mutators);
    }

    public bool Empty => minDamage == -1 && maxDamage == -1 && critRate == -1 && advantageRate == -1;
}


//
// This is a class because I would like to mutate it via a Unit's stats, etc
public class MutableEngagementStats {
    public int minDamage;
    public int maxDamage;
    public int critRate;
    public int advantageRate;
    public List<string> mutators;

    public MutableEngagementStats(Attack a, Defense d) {
        maxDamage = (int)Mathf.Clamp((a.maxDamage   - d.damageReduction), 0f, 99f);
        minDamage = (int)Mathf.Clamp((a.minDamage   - d.damageReduction), 0f, maxDamage);   // cap minDamage by maxDamage
        critRate  = (int)Mathf.Clamp((a.critRate - d.critAvoidRate), 0f, 100f);
        advantageRate = a.dexterity - d.reflex;
        //
        mutators = new List<string>();
    }

    public void AddMutator(IMutatorComponent mc) {
        mutators.Add(mc.displayName);
    }
}