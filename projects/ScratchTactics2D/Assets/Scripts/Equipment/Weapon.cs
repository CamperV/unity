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
        ["A"] = 4.0f,
        ["B"] = 3.0f,
        ["C"] = 2.0f,
        ["D"] = 1.0f
    };
    public abstract string strScaling { get; }

    private Dictionary<string, float> dexScalingValues = new Dictionary<string, float>{
        ["A"] = 3.0f,
        ["B"] = 2.0f,
        ["C"] = 1.0f,
        ["D"] = 0.0f
    };
    public abstract string dexScaling { get; }

    public float strScalingBonus { get => strScalingValues[strScaling]; }
    public float dexScalingBonus { get => dexScalingValues[dexScaling]; }
}
