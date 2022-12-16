using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Items/DiceWeapon")]
public class DiceWeapon : Weapon
{
    public List<Die> dice;

    public override Pair<int, int> DamageRange(Unit thisUnit) {
        int maxDmg = dice.Select(it => it.Max).Sum();
        int minDmg = dice.Select(it => it.Min).Sum();
        int upper = Mathf.Clamp(maxDmg, 0, _MAX_DAMAGE_VALUE);
        int lower = Mathf.Clamp(minDmg, 0, upper);
        return new Pair<int, int>(lower, upper);
    }

    public override int RollDamage(Unit thisUnit) {
        return dice.Select(it => it.Roll()).Sum();
    }

    public override int ComboDamage(Unit thisUnit) {
        return dice.Select(it => it.Min).Sum(); // don't add STRENGTH here
    }

    public override string DisplayRawDamage(Unit thisUnit) {
        Debug.Assert( dice.All(d => d.Name == dice[0].Name) );
        return $"{dice.Count}{dice[0].Name} + STR";
    }

    public override Dictionary<int, float> GenerateProjection(Unit thisUnit) {
        Dictionary<int, float> damageProjection = new Dictionary<int, float>();

        // we abstract this such that we can add "WeightedDie" later
        // but for now, just calculate a base probability via Dia
        foreach (Die die in dice) {
            foreach (int faceValue in die.Faces) {
                int potentialDamage = faceValue;

                // start at 0 and if, over multiple die, we can roll it more, add the prob
                if (!damageProjection.ContainsKey(potentialDamage)) damageProjection[potentialDamage] = 0f;
                damageProjection[potentialDamage] += die.BaseProbability;
            }
        }

        return damageProjection;
    }
}