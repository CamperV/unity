using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class WeaponStats : MonoBehaviour
{
    // natural weapon prototyping
    public int MIN_MIGHT;
    public int MAX_MIGHT;
    public int CRITICAL;
    public int MIN_RANGE;
    public int MAX_RANGE;

    // deprecated, used by MartialWeapons
    public int MIGHT;
    public int ACCURACY;
    public int WEIGHT;
}