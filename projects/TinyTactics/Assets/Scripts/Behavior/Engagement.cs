using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

// this class is created for an acutal battle between two Units
public class Engagement
{
    // public delegate void EngagementCreation(ref Engagement e);
    // public static event EngagementCreation EngagementCreationEvent;

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
            damage   = (int)Mathf.Clamp((a.damage   - d.damageReduction), 0f, 999f);
            hitRate  = (int)Mathf.Clamp((a.hitRate  - d.avoidRate), 0f, 100f);
            critRate = (int)Mathf.Clamp((a.critRate - d.critAvoidRate), 0f, 100f);
		}

        public bool Empty { get => damage == -1 && hitRate == -1 && critRate == -1; }
	}

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
        // EngagementCreationEvent(ref mutableEngagement);

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
        yield return new WaitForSeconds(0.75f);
        ///

        // if we can counterattack:
        if (defenderSurvived && counterAttack != null) {

            // pause again to let the animation finish            
            aggressorSurvived = Process(defender, aggressor, counterAttack.Value, counterDefense.Value, "counter");
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
            return new Stats(-1, -1, -1);
        } else {
            return new Stats(counterAttack.Value, counterDefense.Value);
        }
    }

    private Attack GenerateAttack(Unit generator, Unit target) {
        int weightPenalty = Mathf.Max(0, generator.equippedWeapon.weaponStats.WEIGHT - generator.unitStats.STRENGTH);

        MutableAttack mutableAttack = new MutableAttack(
            generator.unitStats.STRENGTH  + generator.equippedWeapon.weaponStats.MIGHT,         // damage
            generator.unitStats.DEXTERITY*2 + generator.equippedWeapon.weaponStats.ACCURACY,      // hit rate
            0                                                                                   // crit rate
        );
        
        // THIS WILL MODIFY THE OUTGOING ATTACK PACKAGE
        generator.FireOnAttackEvent(ref mutableAttack, target);
        return new Attack(mutableAttack);
    }

    private Defense GenerateDefense(Unit generator, Unit attacker) {
        MutableDefense mutableDefense = new MutableDefense(
            generator.unitStats.DEFENSE,          // reduce incoming damage
            generator.unitStats._AVO,             // avoid rate
            0                                     // crit avoid rate
        );

        // THIS WILL MODIFY THE OUTGOING DEFENSE PACKAGE
        generator.FireOnDefendEvent(ref mutableDefense, attacker);
        return new Defense(mutableDefense);
    }

    private bool Process(Unit A, Unit B, Attack _attack, Defense _defense, string attackType) {
        A.TriggerAttackAnimation(B.gridPosition);
        Stats finalStats = new Stats(_attack, _defense);

        // log the Engagement
        UIManager.inst.combatLog.AddEntry(
            $"{A.logTag}@[{A.displayName}] {attackType}s: [ YELLOW@[{finalStats.damage}] ATK, YELLOW@[{finalStats.hitRate}] HIT, YELLOW@[{finalStats.critRate}] CRIT ]"
        );

		// calc hit/crit
		int diceRoll = Random.Range(0, 100);
		bool isHit = diceRoll <= finalStats.hitRate;

		// final retval
		bool survived = true;
		if (isHit) {
            A.FireOnHitEvent(B);
            A.personalAudioFX.PlayWeaponAttackFX();

			bool isCrit = diceRoll < finalStats.critRate;
            int sufferedDamage = (isCrit) ? finalStats.damage*3 : finalStats.damage;

            // hit/crit
            if (isCrit) UIManager.inst.combatLog.AddEntry("YELLOW@[Critical Hit!]");          

            // ouchies, play the animations for hurt
			survived = B.SufferDamage(sufferedDamage, isCritical: isCrit);

        // miss
		} else {
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