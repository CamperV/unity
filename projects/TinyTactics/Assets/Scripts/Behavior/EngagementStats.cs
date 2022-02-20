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
    public List<string> mutators;

    public EngagementStats(int minDmg, int maxDmg, int cr) {
        minDamage = minDmg;
        maxDamage = maxDmg;
        critRate = cr;
        //
        mutators = new List<string>();
    }

    public EngagementStats(MutableEngagementStats mutES) {
        minDamage = mutES.minDamage;
        maxDamage = mutES.maxDamage;
        critRate = mutES.critRate;
        //
        mutators = new List<string>(mutES.mutators);
    }

    public bool Empty { get => minDamage == -1 && maxDamage == -1 && critRate == -1; }
}


//
// This is a class because I would like to mutate it via a Unit's stats, etc
public class MutableEngagementStats {
    public int minDamage;
    public int maxDamage;
    public int critRate;
    public List<string> mutators;

    public MutableEngagementStats(Attack a, Defense d) {
        maxDamage   = (int)Mathf.Clamp((a.maxDamage   - d.damageReduction), 0f, 999f);
        minDamage   = (int)Mathf.Clamp((a.minDamage   - d.damageReduction), 0f, maxDamage);   // cap minDamage by maxDamage
        critRate = (int)Mathf.Clamp((a.critRate - d.critAvoidRate), 0f, 100f);
        //
        mutators = new List<string>();
    }

    public void AddMutator(IMutatorComponent mc) {
        mutators.Add(mc.displayName);
    }
}