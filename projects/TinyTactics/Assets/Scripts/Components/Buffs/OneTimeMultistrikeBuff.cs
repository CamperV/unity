using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class OneTimeMultistrikeBuff : ValuedStatus
{
    public override string displayName => $"+{modifierValue} Multistrike ({provider})";
    public override string affectedStat => "n/a";

    public override void OnAcquire() {
        boundUnit.OnFinishTurn += ExpireImmediately;
        boundUnit.unitStats.UpdateMultistrike(boundUnit.unitStats._MULTISTRIKE + modifierValue);
    }

    public override void OnExpire() {
        boundUnit.OnFinishTurn -= ExpireImmediately;
        boundUnit.unitStats.UpdateMultistrike(boundUnit.unitStats._MULTISTRIKE - modifierValue);
    }
}