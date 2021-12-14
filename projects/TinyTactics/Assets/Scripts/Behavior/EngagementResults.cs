using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public struct EngagementResults
{
    public Unit aggressor;
    public Unit defender;

    public bool aggressorSurvived;
    public bool defenderSurvived;

    public Attack attack;
    public Attack? counterAttack;

    // this inst is created for EngagementPreviews + simulations
    public EngagementResults(Unit a, Unit b, bool aS, bool dS, Attack _attack, Attack? _counterAttack) {
        aggressor = a;
        defender = b;

        aggressorSurvived = aS;
        defenderSurvived = dS;

        attack = _attack;
        counterAttack = _counterAttack;
    }
}