using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MoveBuff : ValuedStatus
{
    public override string displayName => $"+{modifierValue} Move ({provider})";
    public override string affectedStat => "MOV";

    public override void OnAcquire() {
        boundUnit.OnFinishTurn += TickExpire;

        boundUnit.unitStats.UpdateMove(boundUnit.unitStats.MOVE + modifierValue);
    }

    public override void OnExpire() {
        boundUnit.OnFinishTurn -= TickExpire;

        boundUnit.unitStats.UpdateMove(boundUnit.unitStats.MOVE - modifierValue);
    }
}