using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : Equipment
{
    public abstract int MIGHT    { get; }
    public abstract int ACCURACY { get; }
    public abstract int CRITICAL { get; }
    public abstract int REACH    { get; }

    private Dictionary<string, float> strScalingValues = new Dictionary<string, float>{
        ["A"] = 4.00f,
        ["B"] = 2.00f,
        ["C"] = 1.00f,
        ["D"] = 0.50f
    };
    public abstract string strScaling { get; }

    private Dictionary<string, float> dexScalingValues = new Dictionary<string, float>{
        ["A"] = 2.00f,
        ["B"] = 1.00f,
        ["C"] = 0.50f,
        ["D"] = 0.25f
    };
    public abstract string dexScaling { get; }

    public float strScalingBonus { get => strScalingValues[strScaling]; }
    public float dexScalingBonus { get => dexScalingValues[dexScaling]; }
}
