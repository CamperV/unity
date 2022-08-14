using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

[Serializable]
public struct EngagementStats {
    public DamageContext finalDamageContext;
    public int critRate;
    public int advantageRate;   // this is an integer that represents how many times you can re-roll and take the higher number
    public bool hasAdvantage;
    public bool hasDisadvantage;
    public List<string> mutators;

    public EngagementStats(MutableEngagementStats mutES) {
        finalDamageContext = mutES.finalDamageContext;
        critRate = mutES.critRate;
        advantageRate = mutES.advantageRate;
        hasAdvantage = mutES.HasAdvantage;
        hasDisadvantage = mutES.HasDisadvantage;
        //
        mutators = new List<string>(mutES.mutators);
    }

    public EngagementStats(int cr, int adv) {
        finalDamageContext = new DamageContext(0, null);
        critRate = cr;
        advantageRate = adv;
        hasAdvantage = false;
        hasDisadvantage = false;
        //
        mutators = new List<string>();
    }
    public bool Empty => critRate == -1 && advantageRate == -1;
}


//
// This is a class because I would like to mutate it via a Unit's stats, etc
public class MutableEngagementStats {
    public DamageContext finalDamageContext;
    public int critRate;
    public int advantageRate;
    public List<string> mutators;

        // this can be negative. If negative, you're taking the lower roll
    private readonly int advantageThreshold = 3;
    private int NumRerolls => (int)Mathf.Floor(advantageRate/advantageThreshold);
    public bool StandardRoll => NumRerolls == 0;
    public bool HasAdvantage => NumRerolls > 0;
    public bool HasDisadvantage => NumRerolls < 0;

    // responsible for actually calculating the attack/defense values, including final damage
    public MutableEngagementStats(Attack a, Defense d) {
        critRate  = (int)Mathf.Clamp(a.critRate - d.critAvoidRate, 0f, 100f);
        advantageRate = a.advantageRate - d.advantageRate;

        // update/create the newest DamageContext
        // that includes bonus damage, bonus multipliers, damage reduction, and advantage
        // update both the project and the roller
        Func<int> _DamageResolver = () => (int)Mathf.Clamp(
            RollWithAdvantage(a.damageContext.Resolver) - d.damageReduction, 0f, 99f
        );
        finalDamageContext = new DamageContext(
            ModifyProjection(a.damageContext.Projection, -d.damageReduction),
            _DamageResolver
        );

        //
        mutators = new List<string>();
    }

    public void AddMutator(IMutatorComponent mc) {
        mutators.Add(mc.displayName);
    }

    private Dictionary<int, float> ModifyProjection(Dictionary<int, float> projection, int modifier) {
        Dictionary<int, float> finalProjection = new Dictionary<int, float>();

        // this assumes all damage values are contiguous
        int keyOffset = 1 - projection.Keys.Min();
        int numRolls = 1 + Mathf.Abs(NumRerolls);
        float P_sumPrev = 0f;

        foreach (int dmgKey in projection.Keys.OrderBy(it => it)) {
            int numFaces = (int)(1f/projection[dmgKey]);
            int targetFaceValue = dmgKey + keyOffset;

            float P_target = Mathf.Pow((float)targetFaceValue/(float)numFaces, numRolls) - P_sumPrev;
            finalProjection[dmgKey + modifier] = (StandardRoll || HasAdvantage) ? P_target : 1f - P_target;
            
            Debug.Log($"n={numRolls}, m={numFaces}, T={targetFaceValue}, P={P_target}, sumOfAllPrev: {P_sumPrev}");
            P_sumPrev += P_target;
        }

        return finalProjection;
    }

    private int RollWithAdvantage(Func<int> DamageRoller) {
        string printString = $"Roll: ";
        printString += $"adv: {NumRerolls} => (";
        
        int numRolls = 1 + Mathf.Abs(NumRerolls);

        int highestRoll = Int32.MinValue;
        int lowestRoll = Int32.MaxValue;
        while (numRolls > 0) {
            int rollValue = DamageRoller();
            printString += $" {rollValue} ";

            highestRoll = Mathf.Max(rollValue, highestRoll);
            lowestRoll = Mathf.Min(rollValue, lowestRoll);

            numRolls--;
        }
        
        // if you're at adv/disadv, return different rolls
        // if there IS no advantage, highestRoll == lowestRoll anyway
        int sel = (HasAdvantage) ? highestRoll : lowestRoll;
        //
        printString += $") :: {sel}";
        Debug.Log(printString);
        //
        return sel;
    }
}