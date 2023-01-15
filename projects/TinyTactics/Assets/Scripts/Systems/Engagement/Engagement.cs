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
            for (int s = 0; s < (initiator.statSystem.MULTISTRIKE+1); s++) {
                attacks.Add( Attack.GenerateAttack(initiator, target) );

                // generate potential combo here
                AttackGenerated?.Invoke(initiator, target, ref attacks);
            }

            // then generate all counters if possible
            if (CounterAttackPossible(initiator, target)) {
                for (int s = 0; s < (target.statSystem.MULTISTRIKE+1); s++) {
                    counterAttacks.Add( Attack.GenerateAttack(target, initiator) );

                    // generate potential combo here
                    AttackGenerated?.Invoke(target, initiator, ref counterAttacks);
                }
            }
        }

    }

    public Damage TotalDamage(bool counter = false) {
        if (counter && counterAttacks.Any()) {
            return counterAttacks.Select(ca => ca.damage).Aggregate((a, b) => a + b);
        } else if (attacks.Any()) {
            return attacks.Select(ca => ca.damage).Aggregate((a, b) => a + b);
        } else {
            return new Damage(0);
        } 
    }

    public Damage TotalPoiseDamage(bool counter = false) {
        if (counter && counterAttacks.Any()) {
            return counterAttacks.Select(ca => ca.poiseDamage).Aggregate((a, b) => a + b);
        } else if (attacks.Any()) {
            return attacks.Select(ca => ca.poiseDamage).Aggregate((a, b) => a + b);
        } else {
            return new Damage(0);
        } 
    }

    public Damage TotalDamageTargeting(Unit target) {
        if (!attacks.Any()) return new Damage(0);
        return attacks.Where(a => a.target == target).Select(a => a.damage).Aggregate((a, b) => a + b);
    }

    public Damage TotalPoiseDamageTargeting(Unit target) {
        if (!attacks.Any()) return new Damage(0);
        return attacks.Where(a => a.target == target).Select(a => a.poiseDamage).Aggregate((a, b) => a + b);
    }

    public IEnumerable<Unit> GetUnits() {
        foreach (Unit u in targets) yield return u;
        yield return initiator;
    }

    public static bool CounterAttackPossible(Unit agg, Unit def) {
        TargetRange defenderTargetRange = TargetRange.Standing(
            def.gridPosition,
            def.EquippedWeapon.MIN_RANGE,
            def.EquippedWeapon.MAX_RANGE
        );
        return defenderTargetRange.ValidTarget(agg.gridPosition) && def.statSystem.CounterAttackAvailable;
    }

    public static bool CounterAttackPossible(Unit def, GridPosition fromPosition) {
        TargetRange defenderTargetRange = TargetRange.Standing(
            def.gridPosition,
            def.EquippedWeapon.MIN_RANGE,
            def.EquippedWeapon.MAX_RANGE
        );
        return defenderTargetRange.ValidTarget(fromPosition) && def.statSystem.CounterAttackAvailable;
    }
}