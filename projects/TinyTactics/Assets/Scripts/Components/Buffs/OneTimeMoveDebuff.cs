using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class OneTimeMoveDebuff : ValuedStatus
{
    public override string displayName => $"{modifierValue} Damage ({provider})";
    public override string affectedStat => "MOV";

    public override void OnAcquire() {
        boundUnit.OnFinishTurn += ExpireImmediately;

        boundUnit.unitStats.UpdateMove(boundUnit.unitStats.MOVE + modifierValue);
    }

    public override void OnExpire() {
        boundUnit.OnFinishTurn -= ExpireImmediately;

        boundUnit.unitStats.UpdateMove(boundUnit.unitStats.MOVE - modifierValue);
    }
}