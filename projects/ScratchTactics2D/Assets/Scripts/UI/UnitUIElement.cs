using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public abstract class UnitUIElement : UIElement
{
    [HideInInspector] public UnitUI parentUI;
    public Unit boundUnit { get => parentUI?.boundUnit ?? null; }

    public bool transparencyLock = false;

    public void BindUI(UnitUI UI) {
        Debug.Assert(parentUI == null);
        parentUI = UI;
    }
}