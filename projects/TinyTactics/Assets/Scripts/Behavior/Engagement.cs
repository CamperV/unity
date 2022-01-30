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
        Engagement mutableEngagement = new Engagement(a, b);

        // only return after registered things have modified the Engagement
        return mutableEngagement;
    }

    public static bool CounterAttackPossible(Unit agg, Unit def) {
        AttackRange defenderAttackRange = AttackRange.Standing(
            def.gridPosition,
            def.equippedWeapon.weaponStats.MIN_RANGE,
            def.equippedWeapon.weaponStats.MAX_RANGE
        );
        return defenderAttackRange.ValidAttack(agg.gridPosition);
    }
    public static bool CounterAttackPossible(Unit agg, Unit def, GridPosition fromPosition) {
        AttackRange defenderAttackRange = AttackRange.Standing(
            def.gridPosition,
            def.equippedWeapon.weaponStats.MIN_RANGE,
            def.equippedWeapon.weaponStats.MAX_RANGE
        );
        return defenderAttackRange.ValidAttack(fromPosition);
    }

    public IEnumerator Resolve() {
        resolvedFlag = false;

        bool aggressorSurvived = true;
        bool defenderSurvived = true;

        // animate, then create a little pause before counterattacking
        // ReceiveAttack contains logic for animation processing
        defenderSurvived = Process(aggressor, defender, attack, defense, "attack");
        yield return new WaitForSeconds(0.65f);
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
            return new EngagementStats(-1, -1, -1);
        } else {
            return GenerateEngagementStats(counterAttack.Value, counterDefense.Value);
        }
    }

    private Attack GenerateAttack(Unit generator, Unit defender) {
        MutableAttack mutableAttack = new MutableAttack(
            generator.unitStats._ATK,
            generator.unitStats._HIT,
            generator.unitStats._CRT,
            defender.gridPosition.ManhattanDistance(generator.gridPosition) == 1
        );
        
        // THIS WILL MODIFY THE OUTGOING ATTACK PACKAGE
        generator.FireOnAttackEvent(ref mutableAttack, defender);
        return new Attack(mutableAttack);
    }

    private Defense GenerateDefense(Unit generator, Unit attacker) {
        MutableDefense mutableDefense = new MutableDefense(
            generator.unitStats.DEFENSE,          // reduce incoming damage
            generator.unitStats._AVO,             // avoid rate
            generator.unitStats._CRTAVO,           // crit avoid rate
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
            $"{A.logTag}@[{A.displayName}] {attackType}s: [ YELLOW@[{finalStats.damage}] ATK, YELLOW@[{finalStats.hitRate}] HIT, YELLOW@[{finalStats.critRate}] CRIT ]"
        );

		// calc hit/crit
		int RN1 = Random.Range(0, 100);
		
        // 1 RN
        // bool isHit = RN1 <= finalStats.hitRate;

        // "True Hit"
        // 2RN
        int RN2 = Random.Range(0, 100);
        int trueHitRN = (int)((RN1 + RN2)/2f);
        bool isHit =  trueHitRN <= finalStats.hitRate;

		// final retval
		bool survived = true;
		if (isHit) {
            A.personalAudioFX.PlayWeaponAttackFX();

			bool isCrit = RN1 < finalStats.critRate;
            int sufferedDamage = (isCrit) ? finalStats.damage*3 : finalStats.damage;

            // if the hit is... unimpressive, play a clang or something
            if (sufferedDamage <= 0) A.personalAudioFX.PlayBlockFX();

            // hit/crit
            if (isCrit) {
                A.FireOnCriticalEvent(B);
                A.personalAudioFX.PlayCriticalFX();
                UIManager.inst.combatLog.AddEntry("YELLOW@[Critical Hit!]");          
            }

            // ouchies, play the animations for hurt
			survived = B.SufferDamage(sufferedDamage, isCritical: isCrit);
            
            // fire the event after suffering damage, so the animations are queued in the right order
            // this also means you will not be debuffed or anything if you die
            A.FireOnHitEvent(B);

        // miss
		} else {
            // this is emitted here, because OnAvoid doesn't carry state like "toHit" as it exists here
            B.messageEmitter.Emit(MessageEmitter.MessageType.Miss, $"{finalStats.hitRate}%");

            B.FireOnAvoidEvent();
            B.personalAudioFX.PlayAvoidFX();
		}

		return survived;
	}

	public IEnumerator ExecuteAfterResolving(Action VoidAction) {
        yield return new WaitUntil(() => resolvedFlag == true);
		VoidAction();
	}
}