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
        Attack modifiedAttack = Attack.Modify(firstAttack, aggressor, defender);
        defenderSurvived = defender.ReceiveAttack(modifiedAttack);

        // animate, then create a little pause before counterattacking
        aggressor.BumpTowards(defender.gridPosition, Battle.active.grid, distanceScale: 7.0f);
        yield return new WaitForSeconds(1.0f);

        // if we can counterattack:
        if (defenderSurvived && defender.InStandingAttackRange(aggressor.gridPosition)) {
            Debug.Log($"{defender} counterattacks:");
            Attack counterAttack = defender.GenerateAttack(isAggressor: false);
            Attack modifiedCounterAttack = Attack.Modify(counterAttack, defender, aggressor);
            aggressorSurvived = aggressor.ReceiveAttack(modifiedCounterAttack);

            // pause again to let the animation finish
            defender.BumpTowards(aggressor.gridPosition, Battle.active.grid);
            yield return new WaitForSeconds(1.0f);
        }

        results = new EngagementResults(aggressor, defender, aggressorSurvived, defenderSurvived);
        resolved = true;
    }

    // this previews what will happen, to display, and not resolve
    public EngagementResults SimulateResults() {
        Attack firstAttack = aggressor.GenerateAttack(isAggressor: true);
        Attack modifiedAttack = Attack.Modify(firstAttack, aggressor, defender);

        // if we can counterattack:
        Attack modifiedCounterAttack = null;
        if (defender.InStandingAttackRange(aggressor.gridPosition)) {
            modifiedCounterAttack = Attack.Modify(defender.GenerateAttack(isAggressor: false), defender, aggressor);
        }

        return new EngagementResults(aggressor, defender, modifiedAttack, modifiedCounterAttack ?? null);
    }
    
	public IEnumerator ExecuteAfterResolving(Action VoidAction) {
		while (!resolved) {
			yield return null;
		}
		VoidAction();
	}
}