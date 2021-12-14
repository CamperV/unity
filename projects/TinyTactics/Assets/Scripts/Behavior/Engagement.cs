using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// this class is created for an acutal battle between two Units
public class Engagement
{
    public delegate void EngagementCreation(ref Engagement e);
    public static event EngagementCreation EngagementCreationEvent;

    public Unit aggressor;
    public Unit defender;

    public Attack attack;
    public Attack? counterAttack;

    public EngagementResults engagementResults;
    private bool resolvedFlag = false;


    public Engagement(Unit a, Unit b) {
        aggressor = a;
        defender = b;

        // TODO in the future, the Attacker may have modifications based on current GridPosition or others
        // or, send an Event trigger out with this new Engagement
        attack = aggressor.GenerateAttack();
        
        // if defender can counterattack:
        counterAttack = null;
        if (CounterAttackPossible(a, b)) {
            counterAttack = defender.GenerateAttack();
        }
    }

    public static Engagement Create(Unit a, Unit b) {
        Engagement retVal = new Engagement(a, b);
        // EngagementCreationEvent(ref retVal);

        // only return after registered things have modified the Engagement
        return retVal;
    }

    private static bool CounterAttackPossible(Unit agg, Unit def) {
        AttackRange defenderAttackRange = new AttackRange(
            def.gridPosition,
            def.unitStats.MIN_RANGE,
            def.unitStats.MAX_RANGE
        );
        return defenderAttackRange.ValidAttack(agg.gridPosition);
    }

    // this previews what will happen, to display, and not resolve
    // public EngagementResults Simulate() {
    //     return 
    // }

    public IEnumerator Resolve() {
        resolvedFlag = false;

        bool aggressorSurvived = true;
        bool defenderSurvived = true;

        Debug.Log($"{aggressor} makes first attack: {attack.ToString()}");

        // animate, then create a little pause before counterattacking
        // ReceiveAttack contains logic for animation processing
        aggressor.TriggerAttackAnimation(defender.gridPosition);
        defenderSurvived = defender.ReceiveAttack(attack);
        ///
        yield return new WaitForSeconds(0.25f);
        ///

        // if we can counterattack:
        if (defenderSurvived && counterAttack != null) {
            Debug.Log($"{defender} counterattacks: isnull=={counterAttack==null} {counterAttack.Value.ToString()}");

            // pause again to let the animation finish
            defender.TriggerAttackAnimation(aggressor.gridPosition);
            aggressorSurvived = aggressor.ReceiveAttack(counterAttack.Value);
            ///
            yield return new WaitForSeconds(0.25f);
            ///
        }

        // create the results here, triggering resolution
        engagementResults = new EngagementResults(
            aggressor, defender,
            aggressorSurvived, defenderSurvived,
            attack, counterAttack
        );

        //
        // animate the consequences of the Engagement
        if (!defenderSurvived) defender.TriggerDeathAnimation();
        if (!aggressorSurvived) aggressor.TriggerDeathAnimation();

        if (!defenderSurvived) defender.DeathCleanUp();
        if (!aggressorSurvived) aggressor.DeathCleanUp();

        // why do we do separate checks here?
        // because, if we do defender.DeathCleanUp() before the aggressorSurvived check, 
        // we want these to serially animate, so the user knows what exactly happened
        // this state, in which they simulatneously die, can happen due to something akin to spikes, poison, etc
        resolvedFlag = true;
    }

	public IEnumerator ExecuteAfterResolving(Action VoidAction) {
        yield return new WaitUntil(() => resolvedFlag == true);
		VoidAction();
	}
}