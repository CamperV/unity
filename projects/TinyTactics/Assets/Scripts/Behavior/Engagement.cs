using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

// this class is created for an acutal battle between two Units
public class Engagement
{
    public Unit aggressor;
    public Unit defender;

    public Attack attack;
    public Attack? counterAttack;

    public Defense defense;
    public Defense? counterDefense;

    private bool resolvedFlag = false;

    public Engagement(Unit a, Unit b) {
        aggressor = a;
        defender = b;

        attack = GenerateAttack(aggressor, defender);
        defense = GenerateDefense(defender, aggressor);
        
        // if defender can counterattack:
        counterAttack = null;
        counterDefense = null;
        if (CounterAttackPossible(a, b)) {
            counterAttack = GenerateAttack(defender, aggressor);
            counterDefense = GenerateDefense(aggressor, defender);
        }
    }

    public static Engagement Create(Unit a, Unit b) {
        return new Engagement(a, b);
    }

    public static bool CounterAttackPossible(Unit agg, Unit def) {
        TargetRange defenderTargetRange = TargetRange.Standing(
            def.gridPosition,
            def.EquippedWeapon.MIN_RANGE,
            def.EquippedWeapon.MAX_RANGE
        );
        return defenderTargetRange.ValidTarget(agg.gridPosition) && def.counterAttackAvailable;
    }
    public static bool CounterAttackPossible(Unit agg, Unit def, GridPosition fromPosition) {
        TargetRange defenderTargetRange = TargetRange.Standing(
            def.gridPosition,
            def.EquippedWeapon.MIN_RANGE,
            def.EquippedWeapon.MAX_RANGE
        );
        return defenderTargetRange.ValidTarget(fromPosition) && def.counterAttackAvailable;
    }

    public IEnumerator Resolve() {
        resolvedFlag = false;

        bool aggressorSurvived = true;
        bool defenderSurvived = true;

        List<Unit> comboAllies = defender.EnemiesThreateningCombo().Where(u => u != aggressor).ToList();
        Debug.Log($"has {comboAllies.Count} c.Al");

        // animate, then create a little pause before counterattacking
        // ReceiveAttack contains logic for animation processing
        int numStrikes = 1 + aggressor.unitStats._MULTISTRIKE;
        while (numStrikes > 0 && defenderSurvived) {
            defenderSurvived = ProcessAttack(aggressor, defender, attack, defense);
            //
            numStrikes--;

            if (defenderSurvived) {
                // before we go to the next attack, process ComboAttacks (if defender is still around)
                if (comboAllies.Count > 0) {
                    yield return new WaitForSeconds(0.35f);

                    foreach (Unit comboUnit in comboAllies) {
                        ComboAttack comboAttack = GenerateComboAttack(comboUnit, defender);
                        defenderSurvived = ProcessCombo(comboUnit, defender, comboAttack, defense);

                        yield return new WaitForSeconds(0.35f);
                    }
                }
            }

            yield return (numStrikes == 0) ? new WaitForSeconds(0.65f) : new WaitForSeconds(0.35f);
        }

        yield return new WaitUntil(aggressor.spriteAnimator.EmptyQueue);
        yield return new WaitUntil(defender.spriteAnimator.EmptyQueue);
        ///

        // if we can counterattack:
        if (defenderSurvived && counterAttack != null) {            
            int numCounterStrikes = 1 + defender.unitStats._MULTISTRIKE;
            while (numCounterStrikes > 0 && aggressorSurvived) {
                aggressorSurvived = ProcessAttack(defender, aggressor, counterAttack.Value, counterDefense.Value);            
                ///

                numCounterStrikes--;
                if (numCounterStrikes > 0) yield return new WaitForSeconds(0.35f);
            }
        }
        
        yield return new WaitUntil(AnimationFinished);
        resolvedFlag = true;
    }

    private bool AnimationFinished() => aggressor.spriteAnimator.DoneAnimating() && defender.spriteAnimator.DoneAnimating();

    // this previews what will happen, to display, and not resolve
    public EngagementStats SimulateAttack() {
        return GenerateEngagementStats(attack, defense);
    }

    public EngagementStats SimulateCounterAttack() {
        if (counterAttack == null) {
            return new EngagementStats(-1, -1);
        } else {
            return GenerateEngagementStats(counterAttack.Value, counterDefense.Value);
        }
    }

    private Attack GenerateAttack(Unit generator, Unit defender) {
        DamageContext damageContext = new DamageContext(
            generator.EquippedWeapon.GenerateProjection(generator),     // the overall range of damage that can be done
            () => generator.EquippedWeapon.RollDamage(generator)        // the executor which returns a true final damage value
        );

        MutableAttack mutableAttack = new MutableAttack(
            damageContext,                      // DamageContext to emit a real value and hold info about it
            generator.EquippedWeapon.CRITICAL,  // crit rate of course
            generator.unitStats.DEXTERITY       // advantage rate
        );
        
        // THIS WILL MODIFY THE OUTGOING ATTACK PACKAGE
        generator.FireOnAttackEvent(ref mutableAttack, defender);
        return new Attack(mutableAttack);
    }

    private Defense GenerateDefense(Unit generator, Unit attacker) {
         MutableDefense mutableDefense = new MutableDefense(
            generator.unitStats.DEFENSE,    // reduce incoming damage
            generator.unitStats.REFLEX,     // crit avoid rate
            generator.unitStats.REFLEX      // advantage rate
        );

        // THIS WILL MODIFY THE OUTGOING DEFENSE PACKAGE
        generator.FireOnDefendEvent(ref mutableDefense, attacker);
        return new Defense(mutableDefense);
    }

    private ComboAttack GenerateComboAttack(Unit generator, Unit defender) {
        MutableComboAttack mutableComboAttack = new MutableComboAttack( generator.EquippedWeapon.ComboDamage(generator) );
        
        // THIS WILL MODIFY THE OUTGOING COMBO-ATTACK PACKAGE
        generator.FireOnComboAttackEvent(ref mutableComboAttack, defender);
        return new ComboAttack(mutableComboAttack);
    }

    private EngagementStats GenerateEngagementStats(Attack _attack, Defense _defense) {
        MutableEngagementStats mutableEngagementStats = new MutableEngagementStats(_attack, _defense);
        
        // THIS WILL MODIFY THE FINAL ENGAGEMENT STATS PACKAGE
        aggressor.FireOnFinalEngagementGeneration(ref mutableEngagementStats);
        defender.FireOnFinalEngagementGeneration(ref mutableEngagementStats);
        return new EngagementStats(mutableEngagementStats);
    }

    private bool ProcessAttack(Unit A, Unit B, Attack _attack, Defense _defense) {
        EngagementStats finalStats = GenerateEngagementStats(_attack, _defense);
        
        A.TriggerAttackAnimation(B.gridPosition);       
        int damage = finalStats.finalDamageContext.Resolver();

        bool isCrit = Random.Range(0, 100) <= finalStats.critRate;
        int sufferedDamage = (isCrit) ? damage*2 : damage;

        // now the theatrics
        A.personalAudioFX.PlayWeaponAttackFX();
       
        // if the hit is... unimpressive, play a clang or something
        if (sufferedDamage < 1) B.personalAudioFX.PlayBlockFX();

        // hit/crit
        if (isCrit) {
            A.FireOnCriticalEvent(B);
            A.personalAudioFX.PlayCriticalFX();
            UIManager.inst.combatLog.AddEntry("YELLOW@[Critical Hit!]");          
        }

        // then the meat
        // ouchies, play the animations for hurt
        bool survived = B.SufferDamage(sufferedDamage, A.transform.position, isCritical: isCrit);
        if (survived) B.FireOnHurtByEvent(A);
        
        // fire the event after suffering damage, so the animations are queued in the right order
        // this also means you will not be debuffed or anything if you die
        A.FireOnHitEvent(B);

		return survived;
	}

    private bool ProcessCombo(Unit A, Unit B, ComboAttack _combo, Defense previousDefense) {        
        A.TriggerAttackAnimation(B.gridPosition);

        // now the theatrics
        A.personalAudioFX.PlayWeaponAttackFX();

        int sufferedDamage = (int)Mathf.Clamp((_combo.damage - previousDefense.damageReduction), 0f, 99f);
       
        // if the hit is... unimpressive, play a clang or something
        if (sufferedDamage < 1) B.personalAudioFX.PlayBlockFX();

        // ouchies, play the animations for hurt
        bool survived = B.SufferDamage(sufferedDamage, A.transform.position);
        if (survived) B.FireOnHurtByEvent(A);
        
        // fire the event after suffering damage, so the animations are queued in the right order
        // this also means you will not be debuffed or anything if you die
        A.FireOnHitEvent(B);

		return survived;
	}

	public IEnumerator ExecuteAfterResolving(Action VoidAction) {
        yield return new WaitUntil(() => resolvedFlag == true);
		VoidAction();
	}
}