using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Items/BrawnWeapon")]
public class BrawnWeapon : Weapon
{
    public int MIN_MIGHT;
    public int MAX_MIGHT;

    public override Pair<int, int> DamageRange(Unit thisUnit) {
        int upper = Mathf.Clamp(thisUnit.unitStats.BRAWN + MAX_MIGHT, 0, _MAX_DAMAGE_VALUE);
        int lower = Mathf.Clamp(thisUnit.unitStats.BRAWN + MIN_MIGHT, 0, upper);
        return new Pair<int, int>(lower, upper);
    }

    public override int RollDamage(Unit thisUnit) {
        return Random.Range(MIN_MIGHT, MAX_MIGHT+1) + thisUnit.unitStats.BRAWN;
    }

    public override int ComboDamage(Unit thisUnit) {
        return (int)Mathf.Floor( (MIN_MIGHT + thisUnit.unitStats.BRAWN) / 2 );
    }

    public override string DisplayRawDamage(Unit thisUnit) {
        if (MIN_MIGHT != MAX_MIGHT) {
            return $"{MIN_MIGHT} - {MAX_MIGHT} + BR";
        } else {
            return $"{MIN_MIGHT} + BR";
        }
    }

    public override Dictionary<int, float> GenerateProjection(Unit thisUnit) {
        Dictionary<int, float> damageProjection = new Dictionary<int, float>();

        foreach (int flatValue in Enumerable.Range(MIN_MIGHT, (MAX_MIGHT+1 - MIN_MIGHT))) {
            int potentialDamage = flatValue + thisUnit.unitStats.BRAWN;

            // start at 0 and if, over multiple die, we can roll it more, add the prob
            if (!damageProjection.ContainsKey(potentialDamage)) damageProjection[potentialDamage] = 0f;
            damageProjection[potentialDamage] += (1f / (MAX_MIGHT+1 - MIN_MIGHT));
        }

        return damageProjection;
    }
}