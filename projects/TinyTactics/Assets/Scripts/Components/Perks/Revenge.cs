using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Revenge : Perk
{
    public override void OnAcquire() {
        boundUnit.OnHurt += GainDamageBuff;
        //
        displayName = "Revenge";
    }

    public override void OnRemoval() {
        boundUnit.OnHurt -= GainDamageBuff;
    }

    private void GainDamageBuff() {
        boundUnit.buffManager.AttachBuff<DamageBuff>();
    }
}