using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UnitUI : MonoBehaviour
{
    [SerializeField] private MiniBar_UI healthBar;
    [SerializeField] private SegBar_UI damageReductionBar;
    [SerializeField] private MiniBar_UI breakBar;
    [SerializeField] private StatusBarUI statusBar;

    [SerializeField] private DisplayValue_UI healthValueDisplay;

    private Unit boundUnit;

    void Awake() {
        boundUnit = GetComponentInParent<Unit>();
    }

    void Start() {
        healthBar.AttachTo(boundUnit);
        damageReductionBar.AttachTo(boundUnit);
        breakBar.AttachTo(boundUnit);
        statusBar.AttachTo(boundUnit);

        healthValueDisplay.AttachTo(boundUnit);
    }
}
