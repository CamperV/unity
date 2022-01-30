using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

[Serializable]
public struct EngagementStats {
    public int damage;
    public int hitRate;
    public int critRate;

    public EngagementStats(int d, int hr, int cr) {
        damage = d;
        hitRate = hr;
        critRate = cr;
    }

    public EngagementStats(MutableEngagementStats mutES) {
        damage   = mutES.damage;
        hitRate  = mutES.hitRate;
        critRate = mutES.critRate;
    }

    public bool Empty { get => damage == -1 && hitRate == -1 && critRate == -1; }
}


//
// This is a class because I would like to mutate it via a Unit's stats, etc
public class MutableEngagementStats {
    public int damage;
    public int hitRate;
    public int critRate;

    public MutableEngagementStats(Attack a, Defense d) {
        damage   = (int)Mathf.Clamp((a.damage   - d.damageReduction), 0f, 999f);
        hitRate  = (int)Mathf.Clamp((a.hitRate  - d.avoidRate), 0f, 100f);
        critRate = (int)Mathf.Clamp((a.critRate - d.critAvoidRate), 0f, 100f);
    }
}