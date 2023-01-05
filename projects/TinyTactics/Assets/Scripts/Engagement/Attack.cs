using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

[Serializable]
public struct Attack
{
    // this is used to allow EngagementStats to make a final damage determination
    public DamageContext damageContext;

    public int critRate;
    public int advantageRate;
    //
    public List<MutatorDisplayData> mutators;

    public Attack(MutableAttack mutAtt) {
        damageContext = mutAtt.damageContext;

        critRate = mutAtt.critRate;
        advantageRate = mutAtt.advantageRate;
        //
        mutators = new List<MutatorDisplayData>(mutAtt.mutators);
    }
}

//
// This is a class because I would like to mutate it via a Unit's stats, etc
public class MutableAttack
{
    public DamageContext damageContext;
    private int bonusDamage = 0;
    private float bonusDamageMultiplier = 1f;

    public int critRate;
    public int advantageRate;

    //
    public List<MutatorDisplayData> mutators;

    public MutableAttack(DamageContext dc, int crit, int adv) {
        damageContext = new DamageContext(
            ModifyProjectionBonus(dc.Projection),
            () => (int)(bonusDamageMultiplier * (dc.Resolver() + bonusDamage))
        );

        critRate = crit;
        advantageRate = adv;
        //
        mutators = new List<MutatorDisplayData>();
    }

    public void AddMutator(IMutatorComponent mc) {
        mutators.Add(mc.mutatorDisplayData);
    }

    public void AddBonusDamage(int add) {
        bonusDamage += add;
        RefreshDamageContext();
    }

    public void AddBonusDamageMultiplier(float mult) {
        bonusDamageMultiplier = mult;
        RefreshDamageContext();
    }

    private void RefreshDamageContext() {
        damageContext = new DamageContext(
            ModifyProjectionBonus(damageContext.Projection),
            damageContext.Resolver
        );
    }

    private Dictionary<int, float> ModifyProjectionBonus(Dictionary<int, float> projection) {
        Dictionary<int, float> finalProjection = new Dictionary<int, float>();

        foreach (int dmgKey in projection.Keys.OrderBy(it => it)) {
            finalProjection[(int)bonusDamageMultiplier*(dmgKey + bonusDamage)] = projection[dmgKey];
        }

        return finalProjection;
    }
}