using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class HealthPickup : MapItem
{
    public int value;

    public override void OnUnitEnter(Unit unit) {
        unit.HealAmount(value);
        Debug.Log($"healed {unit} by {value}");
        Destroy(gameObject);
    }
}
