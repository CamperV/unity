using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UnitUI : MonoBehaviour
{
    [SerializeField] private MiniBar_UI healthBar;
    [SerializeField] private MiniBar_UI breakBar;

    private Unit boundUnit;

    public void Update() {
        // if (boundUnit != null) {
        //     transform.position = Camera.main.WorldToScreenPoint(boundUnit.transform.position);
        // }
    }

	public void AttachTo(Unit unit) {
        healthBar.AttachTo(unit);
        breakBar.AttachTo(unit);

        boundUnit = unit;
    }
}
