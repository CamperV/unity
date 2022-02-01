using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Extensions;

public class EncouragingThumbsUp : Perk, IToolTip
{
    public override string displayName { get; set; } = "Encouraging Thumbs Up From Across the Battlefield";

    // IToolTip
    public string tooltipName { get; set; } = "Encouraging Thumbs Up From Across the Battlefield";
    public string tooltip { get; set; } = "On start of turn, heal a random damaged ally by 10% of their Max HP.";

    public override void OnAcquire() {
        boundUnit.OnStartTurn += HealAlly;
    }

    public override void OnRemoval() {
        boundUnit.OnStartTurn -= HealAlly;
    }

    private void HealAlly(Unit _) {
        var damagedUnits = boundUnit.Allies().Where(u => u.unitStats._CURRENT_HP < u.unitStats.VITALITY);
        if (damagedUnits.Any()) {
            Unit luckyUnit = damagedUnits.ToArray().SelectRandom<Unit>();

            int healAmount = (int)Mathf.Ceil(0.1f * luckyUnit.unitStats.VITALITY);
            luckyUnit.HealAmount(healAmount);
        }
    }
}