using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

// this class is created for an acutal battle between two Units
public class Engagement
{
    public delegate void EngagementCreation(ref Engagement e);
    public static event EngagementCreation EngagementCreationEvent;

    public Unit aggressor;
    public Unit defender;

    public Attack attack;
    public Attack? counterAttack;

    public Defense defense;
    public Defense? counterDefense;

    private bool resolvedFlag = false;

    public struct Stats {
		public int damage;
        public int hitRate;
        public int critRate;

        public Stats(int d, int hr, int cr) {
            damage = d;
            hitRate = hr;
            critRate = cr;
        }

		public Stats(Attack a, Defense d) {
            damage   = a.damage   - d.damageReduction;
            hitRate  = a.hitRate  - d.dodgeRate;
            critRate = a.critRate - d.critDodgeRate;
		}
	}

    public Engagement(Unit a, Unit b) {
        aggressor = a;
        defender = b;

        // TODO in the future, the Attacker may have modifications based on current GridPosition or others
        // or, send an Event trigger out with this new Engagement
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
        // EngagementCreationEvent(ref mutableEngagement);

        // only return after registered things have modified the Engagement
        return mutableEngagement;
    }

    private static bool CounterAttackPossible(Unit agg, Unit def) {
        AttackRange defenderAttackRange = AttackRange.Standing(
            def.gridPosition,
            def.unitStats.MIN_RANGE,
            def.unitStats.MAX_RANGE
        );
        return defenderAttackRange.ValidAttack(agg.gridPosition);
    }

    public IEnumerator Resolve() {
        resolvedFlag = false;

        bool aggressorSurvived = true;
        bool defenderSurvived = true;

        // animate, then create a little pause before counterattacking
        // ReceiveAttack contains logic for animation processing
        defenderSurvived = Process(aggressor, defender, attack, defense);
        yield return new WaitForSeconds(0.75f);
        ///

        // if we can counterattack:
        if (defenderSurvived && counterAttack != null) {

            // pause again to let the animation finish            
            aggressorSurvived = Process(defender, aggressor, counterAttack.Value, counterDefense.Value);
            yield return new WaitForSeconds(0.75f);
            ///
        }

        yield return new WaitUntil( () => !aggressor.spriteAnimator.isAnimating && !defender.spriteAnimator.isAnimating );
        resolvedFlag = true;
    }

    // this previews what will happen, to display, and not resolve
    public Stats SimulateAttack() {
        return new Stats(attack, defense);
    }

    public Stats SimulateCounterAttack() {
        if (counterAttack == null) {
            return new Stats(0, 0, 0);
        } else {
            return new Stats(counterAttack.Value, counterDefense.Value);
        }
    }

    public Attack GenerateAttack(Unit generator, Unit target) {
        MutableAttack mutableAttack = new MutableAttack(
            generator.unitStats.STRENGTH,         // damage
            generator.unitStats.DEXTERITY * 10,   // hit rate
            0                                     // crit rate
        );
        
        // THIS WILL MODIFY THE OUTGOING ATTACK PACKAGE
        generator.FireOnAttackEvent(ref mutableAttack, target);
        return new Attack(mutableAttack);
    }

    public Defense GenerateDefense(Unit generator, Unit attacker) {
        MutableDefense mutableDefense = new MutableDefense(
            generator.unitStats.DAMAGE_REDUCTION, // reduce incoming damage
            generator.unitStats.REFLEX * 10,      // dodge rate
            0                                     // crit avoid rate
        );

        // THIS WILL MODIFY THE OUTGOING DEFENSE PACKAGE
        generator.FireOnDefendEvent(ref mutableDefense, attacker);
        return new Defense(mutableDefense);
    }

    private bool Process(Unit A, Unit B, Attack _attack, Defense _defense) {
        A.TriggerAttackAnimation(B.gridPosition);

        Stats finalStats = new Stats(_attack, _defense);

		// calc hit/crit
		int diceRoll = Random.Range(0, 100);
		bool isHit = diceRoll < finalStats.hitRate;

		// final retval
		bool survived = true;
		if (isHit) {
			bool isCrit = diceRoll < finalStats.critRate;
            int sufferedDamage = (isCrit) ? finalStats.damage*3 : finalStats.damage;

            // ouchies, play the animations for hurt
            B.TriggerHurtAnimation(isCritical: isCrit);
			survived = B.SufferDamage(sufferedDamage);
			Debug.Log($"{B} was hit ({finalStats.hitRate}% to hit), dmg: {sufferedDamage}");

        // miss
		} else {
            B.TriggerMissAnimation();
			Debug.Log($"{B} dodged the attack! ({finalStats.hitRate}% to hit)");
		}

		return survived;
	}

	public IEnumerator ExecuteAfterResolving(Action VoidAction) {
        yield return new WaitUntil(() => resolvedFlag == true);
		VoidAction();
	}
}