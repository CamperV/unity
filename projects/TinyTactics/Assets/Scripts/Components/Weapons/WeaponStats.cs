using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class WeaponStats : MonoBehaviour
{
    public int MIGHT;           // added to STRENGTH for damage
    public int ACCURACY;        // added to DEXTERITY for hitRate
    public int WEIGHT;          // Max(0, WEIGHT - STRENGTH) is subtracted from REFLEX for dodgeRate
    public int MIN_RANGE;
    public int MAX_RANGE;
}