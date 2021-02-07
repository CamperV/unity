using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class EngagementResults
{
    public bool resolved { get; private set; }

    public Unit aggressor;
    public Unit defender;

    public bool aggressorSurvived;
    public bool defenderSurvived;

    public Attack firstAttack;
    public Attack secondAttack;

    // this class is created for an acutal battle between two Units
    public EngagementResults(Unit a, Unit b, bool aS, bool dS) {
        resolved = false;
        
        aggressor = a;
        defender = b;
        aggressorSurvived = aS;
        defenderSurvived = dS;
    }

    // this inst is created for EngagementPreviews
    public EngagementResults(Unit a, Unit b, Attack attack, Attack counterAttack) {
        resolved = false;
        
        aggressor = a;
        defender = b;
        aggressorSurvived = true;
        defenderSurvived = true;
        firstAttack = attack;
        secondAttack = counterAttack;
    }

    public IEnumerator ResolveCasualties() {
        if (!defenderSurvived) {
            defender.TriggerDeathAnimation();
            while (defender.IsAnimating()) yield return null;
        }
        if (!aggressorSurvived) {
            aggressor.TriggerDeathAnimation();
            while (aggressor.IsAnimating()) yield return null;
        }

        if (!defenderSurvived) defender.DeathCleanUp();
        if (!aggressorSurvived) aggressor.DeathCleanUp();

        // why do we do separate checks here?
        // because, if we do defender.DeathCleanUp() before the aggressorSurvived check, 
        // Battle/PhaseManager will end the activeBattle before the aggressor actually dies
        // also, we want these to serially animate, so the user knows what exactly happened
        // this state, in which they simulatneously die, can happen due to something akin to spikes, poison, etc
        resolved = true;
    }

	public IEnumerator ExecuteAfterResolving(Action VoidAction) {
		while (!resolved) {
			yield return null;
		}
		VoidAction();
	}
}