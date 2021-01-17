using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Engagement
{
    public Unit aggressor;
    public Unit defender;

    public bool resolved { get; private set; }
    public EngagementResults results { get; private set; }

    // this class is created for an acutal battle between two Units
    public Engagement(Unit a, Unit b) {
        resolved = false;

        aggressor = a;
        defender = b;
    }

    public IEnumerator ResolveResults() {
        Debug.Assert(resolved == false);
        bool aggressorSurvived = true;
        bool defenderSurvived = true;

        // modify the attack based on inter-Unit properties,
        // such as weapon triangle, etc
        Debug.Log($"{aggressor} makes first attack:");
        Attack firstAttack = aggressor.GenerateAttack(isAggressor: true);
        firstAttack.Modify(aggressor, defender);
        defenderSurvived = defender.ReceiveAttack(firstAttack);

        // create a little pause before counterattacking
        yield return new WaitForSeconds(1.0f);

        // if we can counterattack:
        if (defenderSurvived && defender.InStandingAttackRange(aggressor.gridPosition)) {
            Debug.Log($"{defender} counterattacks:");
            Attack counterAttack = defender.GenerateAttack(isAggressor: false);
            counterAttack.Modify(defender, aggressor);
            aggressorSurvived = aggressor.ReceiveAttack(counterAttack);

            // pause again to let the animation finish
            yield return new WaitForSeconds(1.0f);
        }

        results = new EngagementResults(aggressor, defender, aggressorSurvived, defenderSurvived);
        resolved = true;
    }

    // this previews what will happen, to display, and not resolve
    public EngagementResults PreviewResults() {
        Attack firstAttack = aggressor.GenerateAttack(isAggressor: true);
        firstAttack.Modify(aggressor, defender);

        // if we can counterattack:
        Attack counterAttack = null;
        if (defender.InStandingAttackRange(aggressor.gridPosition)) {
            counterAttack = defender.GenerateAttack(isAggressor: false);
            counterAttack.Modify(defender, aggressor);
        }

        return new EngagementResults(aggressor, defender, firstAttack, counterAttack ?? null);
    }
    
	public IEnumerator ExecuteAfterResolving(Action VoidAction) {
		while (!resolved) {
			yield return null;
		}
		VoidAction();
	}
}