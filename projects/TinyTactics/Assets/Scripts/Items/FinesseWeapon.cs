using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Items/FinesseWeapon")]
public class FinesseWeapon : Weapon
{
    public int MIGHT;

    public override Pair<int, int> DamageRange(Unit thisUnit) {
        int val = Mathf.Clamp(thisUnit.unitStats.FINESSE + MIGHT, 0, _MAX_DAMAGE_VALUE);
        return new Pair<int, int>(val, val);
    }

    public override int RollDamage(Unit thisUnit) {
        return MIGHT + thisUnit.unitStats.FINESSE;
    }

    public override int ComboDamage(Unit thisUnit) {
        return (int)Mathf.Floor( (MIGHT + thisUnit.unitStats.FINESSE) / 2 );
    }

    public override string DisplayRawDamage(Unit thisUnit) {
        return $"{MIGHT} + FN";
    }

    public override Dictionary<int, float> GenerateProjection(Unit thisUnit) {
        Dictionary<int, float> damageProjection = new Dictionary<int, float>();
        damageProjection[RollDamage(thisUnit)] = 1f;
        return damageProjection;
    }
}