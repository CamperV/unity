using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Extensions;
using TMPro;

public class WeaponBadge : MonoBehaviour
{
    [SerializeField] private Image weaponBadgeVisualization;

    // other
    private Unit boundUnit;

    void Awake() {
        boundUnit = GetComponent<Unit>();
    }

    void Start() {
        weaponBadgeVisualization.sprite = boundUnit.equippedWeapon.sprite;
        weaponBadgeVisualization.color = boundUnit.equippedWeapon.color;
    }
}
