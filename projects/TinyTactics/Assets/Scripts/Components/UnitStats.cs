using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Serialization;

public class UnitStats : MonoBehaviour
{
    // publicly visible events for UIs etc to key off of
	public delegate void StatRatioChange(int newValue, int maxValue);
    public event StatRatioChange UpdateHPEvent;

    public delegate void StatChange(int newValue);
    public event StatChange UpdateReflexEvent;

    public int VITALITY;
    public int STRENGTH;
    public int DEXTERITY;
    public int REFLEX;
    public int DAMAGE_REDUCTION;
    public int MOVE;

    public int _CURRENT_HP;

    public void UpdateHP(int newValue, int maxValue) {
        _CURRENT_HP = Mathf.Clamp(newValue, 0, maxValue);
        UpdateHPEvent?.Invoke(_CURRENT_HP, maxValue);
    }

    public void UpdateReflex(int newValue) {
        // REFLEX = Mathf.Clamp(newValue, 0, 99);
        REFLEX = newValue;
        UpdateReflexEvent?.Invoke(newValue);
    }
}