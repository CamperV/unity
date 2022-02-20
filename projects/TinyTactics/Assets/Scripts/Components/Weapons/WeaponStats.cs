using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class WeaponStats : MonoBehaviour
{
    public int MIGHT;           // added to STRENGTH for damage
    public int ACCURACY;        // added to DEXTERITY for hitRate
    public int CRITICAL;
    public int WEIGHT;          // Max(0, WEIGHT - STRENGTH) is subtracted from REFLEX for avoidRate
    public int MIN_RANGE;
    public int MAX_RANGE;

    // natural weapon prototyping
    public int MIN_MIGHT;
    public int MAX_MIGHT;
}