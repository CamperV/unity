using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class EngagementResults
{
    public bool resolved { get; private set; }

    private Unit aggressor;
    private Unit defender;

    public bool aggressorSurvived;
    public bool defenderSurvived;

    // this class is created for an acutal battle between two Units
    public EngagementResults(Unit a, Unit b, bool aS, bool dS) {
        resolved = false;
        
        aggressor = a;
        defender = b;
        aggressorSurvived = aS;
        defenderSurvived = dS;
    }

    public IEnumerator ResolveCasualties() {
        if (!defenderSurvived) defender.Die();
        if (!aggressorSurvived) aggressor.Die();

        // add another while loop if you want two distinct animations
        // for now, we'll kill them simultaneously
        while (aggressor.IsAnimating() || defender.IsAnimating()) yield return null;
        resolved = true;
    }

	public IEnumerator ExecuteAfterResolving(Action VoidAction) {
		while (!resolved) {
			yield return null;
		}
		VoidAction();
	}
}