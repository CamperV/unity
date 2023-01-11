using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Items/GenericWeapon")]
public class GenericWeapon : Weapon
{
    public int MIN_ATK;
    public int MAX_ATK;
    public int COMBO_ATK;

    public int POISE_ATK = 1;
    // public int MIN_RANGE;
    // public int MAX_RANGE;
    // public int CRITICAL;

    public override Pair<int, int> DamageRange(Unit thisUnit) {
        int upper = Mathf.Clamp(MAX_ATK, 0, _MAX_DAMAGE_VALUE);
        int lower = Mathf.Clamp(MIN_ATK, 0, upper);
        return new Pair<int, int>(lower, upper);
    }

    public override int RollDamage(Unit thisUnit) => Random.Range(MIN_ATK, MAX_ATK+1);
    public override int ComboDamage(Unit thisUnit) => COMBO_ATK;
    public override string DisplayRawDamage(Unit thisUnit) => (MIN_ATK != MAX_ATK) ? $"{MIN_ATK} - {MAX_ATK}" : $"{MIN_ATK}";

    public override Dictionary<int, float> GenerateProjection(Unit thisUnit) {
        Dictionary<int, float> damageProjection = new Dictionary<int, float>();

        foreach (int flatValue in Enumerable.Range(MIN_ATK, (MAX_ATK+1 - MIN_ATK))) {
            int potentialDamage = flatValue;

            // start at 0 and if, over multiple die, we can roll it more, add the prob
            if (!damageProjection.ContainsKey(potentialDamage)) damageProjection[potentialDamage] = 0f;
            damageProjection[potentialDamage] += (1f / (MAX_ATK+1 - MIN_ATK));
        }

        return damageProjection;
    }
}