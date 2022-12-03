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

    void Awake() {
        boundUnit = GetComponentInParent<Unit>();
    }

    void Start() {
        healthBar.AttachTo(boundUnit);
        breakBar.AttachTo(boundUnit);
    }
}
