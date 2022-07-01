using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Reckless : Perk, IToolTip
{
    public override string displayName { get; set; } = "Reckless";

    // IToolTip
    public string tooltipName { get; set; } = "Reckless";
    public string tooltip => "Deal and receive +7 additional damage.";

    public override void OnAcquire() {
        boundUnit.OnAttack += OffensiveAdv;
        boundUnit.OnDefend += DefensiveAdv;
    }

    public override void OnRemoval() {
        boundUnit.OnAttack -= OffensiveAdv;
        boundUnit.OnDefend -= DefensiveAdv;
    }

    private void OffensiveAdv(Unit thisUnit, ref MutableAttack mutAtt, Unit target) {
        mutAtt.AddDamage(7);
        mutAtt.AddMutator(this);
    }

    // since "damageReduction" can become negative, we can use this to increase the opponents damage
    private void DefensiveAdv(Unit thisUnit, ref MutableDefense mutDef, Unit target) {
        mutDef.AddDamageReduction(-7);
        mutDef.AddMutator(this);
    }
}