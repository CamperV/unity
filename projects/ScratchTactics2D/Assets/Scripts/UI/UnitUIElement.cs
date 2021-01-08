using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UnitUIElement : MonoBehaviour
{
    [HideInInspector] UnitUI parentUI;
    public Unit boundUnit { get => parentUI?.boundUnit ?? null; }

    public bool transparencyLock = false;

    public void BindUI(UnitUI UI) {
        Debug.Assert(parentUI == null);
        parentUI = UI;
        transform.parent = UI.transform;
    }
}