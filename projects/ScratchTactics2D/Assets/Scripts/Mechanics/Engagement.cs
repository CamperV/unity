using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Engagement
{
    private Unit aggressor;
    private Unit defender;

    public Tuple<bool, bool> results { get; private set; }

    // this class is created for an acutal battle between two Units
    public Engagement(Unit a, Unit b) {
        aggressor = a;
        defender = b;
    }

    public IEnumerator Start() {
        bool aggressorSurvived = true;
        bool defenderSurvived = true;

        // modify the attack based on inter-Unit properties,
        // such as weapon triangle, etc
        Debug.Log($"{aggressor} makes first attack:");
        Attack firstAttack = aggressor.GenerateAttack(isAggressor: true);
        firstAttack.Modify(aggressor, defender);
        defenderSurvived = defender.ReceiveAttack(firstAttack);

        // create a little pause before counterattacking
        while (defender.IsAnimating()) yield return new WaitForSeconds(1.0f);

        // if we can counterattack:
        if (defenderSurvived && defender.InStandingAttackRange(aggressor.gridPosition)) {
            Debug.Log($"{defender} counterattacks:");
            Attack counterAttack = defender.GenerateAttack(isAggressor: false);
            counterAttack.Modify(defender, aggressor);
            aggressorSurvived = aggressor.ReceiveAttack(counterAttack);

            // pause again to let the animation finish
            while (aggressor.IsAnimating()) yield return null;
        }

        results = new Tuple<bool, bool>(aggressorSurvived, defenderSurvived);
    }

	public IEnumerator ExecuteAfterResolving(Action VoidAction) {
		while (results == null) {
			yield return null;
		}
		VoidAction();
	}
}