using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Nimble : Perk
{
    public override void OnAcquire() {
        boundUnit.unitPathfinder.unitPassThroughOverride = true;
        //
        displayName = "Nimble";
    }

    public override void OnRemoval() {
        boundUnit.unitPathfinder.unitPassThroughOverride = false;
    }
}