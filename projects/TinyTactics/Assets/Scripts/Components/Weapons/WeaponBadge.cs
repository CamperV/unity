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
        // weaponBadgeVisualization.sprite = boundUnit.equippedWeapon.sprite;
        if (weaponBadgeVisualization.gameObject.activeInHierarchy) {
            weaponBadgeVisualization.color = boundUnit.equippedWeapon.color;
        }
        // weaponBadgeVisualization.color = boundUnit.equippedWeapon.color.WithSaturation(0.25f);
        // weaponBadgeVisualization.color = Color.Lerp(boundUnit.spriteRenderer.color, boundUnit.equippedWeapon.color, 0.65f).WithAlpha(1f);
    }
}
