using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

// this class is created for an acutal battle between two Units
public class Engagement
{
    public delegate void EngagementAttackEvent(Unit A, Unit B, ref List<Attack> attackList);
    public static event EngagementAttackEvent AttackGenerated;

    // an Engagement ALWAYS procedes:
    // 1) exhaust attacks
    // 2) exhaust counterattacks
    public List<Attack> attacks;
    public List<Attack> counterAttacks;

    public Unit initiator; // A is always the initiator
    public List<Unit> targets; // B is always the defender

    public Engagement(Unit a, Unit b) {
        initiator = a;
        targets = new List<Unit>{b};

        attacks = new List<Attack>();
        counterAttacks = new List<Attack>();

        // receive a counter from each target
        // man that's rough though
        // TODO: use the EquippedWeapon/Unit to maybe create an AoE to grab multiple targets
        foreach (Unit target in targets) {
            
            // gather all attacks from Initiator
            // units that can Combo will add them to this List
            foreach (Attack attack in initiator.GenerateAttacks(target, Attack.AttackType.Normal, Attack.AttackDirection.Normal)) {
                attacks.Add(attack);

                // generate potential combo here
                Debug.Log($"{initiator} generated {attack}");
                AttackGenerated?.Invoke(initiator, target, ref attacks);
            }

            // then generate all counters if possible
            if (CounterAttackPossible(target, initiator.gridPosition)) {
                foreach (Attack counterAttack in target.GenerateAttacks(initiator, Attack.AttackType.Normal, Attack.AttackDirection.Counter)) {
                    counterAttacks.Add(counterAttack);

                    // generate potential combo here
                    AttackGenerated?.Invoke(target, initiator, ref counterAttacks);
                }
            }
        }

    }

    public Damage TotalDamageTargeting(
        Unit target,
        Attack.AttackDirection attackDirection = Attack.AttackDirection.Normal, // Normal/Counter
        Damage.DamageType damageType = Damage.DamageType.Normal                 // Normal/Poise
    ) {
        Damage totalDamage = new Damage(0, _damageType: damageType);

        // return every attack, from all lists
        foreach (Attack a in GetAttacks()) {
            if (a.target != target) continue;
            if (a.attackDirection != attackDirection) continue;

            totalDamage += (damageType == Damage.DamageType.Normal) ? a.damage : a.poiseDamage;
        }
        return totalDamage;
    }

    public IEnumerable<Attack> GetAttacks() {
        foreach (Attack a in attacks) yield return a;
        foreach (Attack ca in counterAttacks) yield return ca;
    }

    public IEnumerable<Attack> GetAttacks(Attack.AttackDirection attackDirection) {
        if (attackDirection == Attack.AttackDirection.Normal) {
            foreach (Attack a in attacks) yield return a;
        } else {
            foreach (Attack ca in counterAttacks) yield return ca;
        }
    }

    public IEnumerable<Unit> GetUnits() {
        foreach (Unit u in targets) yield return u;
        yield return initiator;
    }

    public static bool CounterAttackPossible(Unit unit, GridPosition targetPosition) {
        TargetRange targetRange = TargetRange.Standing(
            unit.gridPosition,
            unit.EquippedWeapon.MIN_RANGE,
            unit.EquippedWeapon.MAX_RANGE
        );
        return targetRange.ValidTarget(targetPosition) && unit.statSystem.CounterAttackAvailable;
    }
}