using System.Collections;
using System.Collections.Generic;
using System;
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
            def.equippedWeapon.MIN_RANGE,
            def.equippedWeapon.MAX_RANGE
        );
        return defenderTargetRange.ValidTarget(agg.gridPosition) && def.counterAttackAvailable;
    }
    public static bool CounterAttackPossible(Unit agg, Unit def, GridPosition fromPosition) {
        TargetRange defenderTargetRange = TargetRange.Standing(
            def.gridPosition,
            def.equippedWeapon.MIN_RANGE,
            def.equippedWeapon.MAX_RANGE
        );
        return defenderTargetRange.ValidTarget(fromPosition) && def.counterAttackAvailable;
    }

    public IEnumerator Resolve() {
        resolvedFlag = false;

        bool aggressorSurvived = true;
        bool defenderSurvived = true;

        // animate, then create a little pause before counterattacking
        // ReceiveAttack contains logic for animation processing
        int numStrikes = 1 + aggressor.unitStats._MULTISTRIKE;
        while (numStrikes > 0 && defenderSurvived) {
            defenderSurvived = Process(aggressor, defender, attack, defense, "attack");
            yield return new WaitForSeconds(0.65f);
            yield return new WaitUntil(aggressor.spriteAnimator.EmptyQueue);
            //
            numStrikes--;
        }
        ///

        // if we can counterattack:
        if (defenderSurvived && counterAttack != null) {
            yield return new WaitUntil(defender.spriteAnimator.EmptyQueue);

            // pause again to let the animation finish            
            aggressorSurvived = Process(defender, aggressor, counterAttack.Value, counterDefense.Value, "counter");
            ///
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
            return new EngagementStats(-1, -1, -1, -1);
        } else {
            return GenerateEngagementStats(counterAttack.Value, counterDefense.Value);
        }
    }

    private Attack GenerateAttack(Unit generator, Unit defender) {
        Pair<int, int> dmgRange = generator.equippedWeapon.DamageRange(generator);

        MutableAttack mutableAttack = new MutableAttack(
            dmgRange.First,
            dmgRange.Second,
            generator.equippedWeapon.CRITICAL,
            generator.unitStats.DEXTERITY,
            defender.gridPosition.ManhattanDistance(generator.gridPosition) == 1
        );
        
        // THIS WILL MODIFY THE OUTGOING ATTACK PACKAGE
        generator.FireOnAttackEvent(ref mutableAttack, defender);
        return new Attack(mutableAttack);
    }

    private Defense GenerateDefense(Unit generator, Unit attacker) {
         MutableDefense mutableDefense = new MutableDefense(
            generator.unitStats.DEFENSE,                      // reduce incoming damage
            0,           // crit avoid rate
            generator.unitStats.REFLEX,
            attacker.gridPosition.ManhattanDistance(generator.gridPosition) == 1
        );

        // THIS WILL MODIFY THE OUTGOING DEFENSE PACKAGE
        generator.FireOnDefendEvent(ref mutableDefense, attacker);
        return new Defense(mutableDefense);
    }

    private EngagementStats GenerateEngagementStats(Attack _attack, Defense _defense) {
        MutableEngagementStats mutableEngagementStats = new MutableEngagementStats(_attack, _defense);
        
        // THIS WILL MODIFY THE FINAL ENGAGEMENT STATS PACKAGE
        aggressor.FireOnFinalEngagementGeneration(ref mutableEngagementStats);
        defender.FireOnFinalEngagementGeneration(ref mutableEngagementStats);
        return new EngagementStats(mutableEngagementStats);
    }

    private bool Process(Unit A, Unit B, Attack _attack, Defense _defense, string attackType) {
        EngagementStats finalStats = GenerateEngagementStats(_attack, _defense);
        
        A.TriggerAttackAnimation(B.gridPosition);

        // log the Engagement
        UIManager.inst.combatLog.AddEntry(
            $"{A.logTag}@[{A.displayName}] {attackType}s: YELLOW@[{finalStats.minDamage}]-YELLOW@[{finalStats.maxDamage}] ATK, YELLOW@[{finalStats.critRate}] CRIT ]"
        );
        
        bool isCrit = Random.Range(0, 100) <= finalStats.critRate;

        // this is the final Damage, linear relationship
        int damage = CalculateFinalDamage(finalStats);
        Debug.Log($"Resulting damage: {damage}");

        int sufferedDamage = (isCrit) ? damage*2 : damage;

        // now the theatrics
        A.personalAudioFX.PlayWeaponAttackFX();
       
        // if the hit is... unimpressive, play a clang or something
        if (sufferedDamage < 1) A.personalAudioFX.PlayBlockFX();

        // hit/crit
        if (isCrit) {
            A.FireOnCriticalEvent(B);
            A.personalAudioFX.PlayCriticalFX();
            UIManager.inst.combatLog.AddEntry("YELLOW@[Critical Hit!]");          
        }

        // then the meat
        // ouchies, play the animations for hurt
        bool survived = B.SufferDamage(sufferedDamage, isCritical: isCrit);
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

    private int CalculateFinalDamage(EngagementStats finalStats) {
        // 1) Linear
        // return Random.Range(finalStats.minDamage, finalStats.maxDamage+1);

        // 2) Advantage-based
        int advantageThreshold = 2;

        int numRolls = 1 + (int)Mathf.Floor((Mathf.Abs(finalStats.advantageRate) / advantageThreshold));
        Debug.Log($"Advantage rate: {finalStats.advantageRate}, numRolls: {numRolls}");

        int highestRoll = Int32.MinValue;
        int lowestRoll = Int32.MaxValue;
        while (numRolls > 0) {
            // roll here
            int rollValue = Random.Range(finalStats.minDamage, finalStats.maxDamage+1);
            Debug.Log($"Rolled {rollValue}");

            highestRoll = Mathf.Max(rollValue, highestRoll);
            lowestRoll = Mathf.Min(rollValue, lowestRoll);

            numRolls--;
        }

        Debug.Log($"Highest: {highestRoll}");
        Debug.Log($"Lowest: {lowestRoll}");

        // if you're at adv/disadv, return different rolls
        return (finalStats.advantageRate > 0) ? highestRoll : lowestRoll;
    }
}