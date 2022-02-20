using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ConditionalAvoidDebuff : ConditionalBuff
{
    public override string displayName => $"{modifierValue} Avoid ({provider})";
    // public override string affectedStat => "";

    public override void OnAcquire() {
        boundUnit.unitMap.NewBoardStateEvent += CheckCondition;
        boundUnit.OnDefend += DebuffAvoid;
    }

    public override void OnExpire() {
        boundUnit.unitMap.NewBoardStateEvent -= CheckCondition;
        boundUnit.OnDefend -= DebuffAvoid;
    }

    private void DebuffAvoid(ref MutableDefense mutDef, Unit target) {
        if (ConditionValid()) {
            mutDef.avoidRate += modifierValue;  // should be negative
            mutDef.AddMutator(this);
        }
    }
}