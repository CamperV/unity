using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Serialization;

public class UnitStats : MonoBehaviour
{
    // publicly visible events for UIs etc to key off of
	public delegate void StatChange(int newValue, int maxValue);
    public event StatChange UpdateHPEvent;

    public int VITALITY;
    public int STRENGTH;
    public int DEXTERITY;
    public int REFLEX;
    public int MOVE;
    public int MIN_RANGE;
    public int MAX_RANGE;

    public int _CURRENT_HP;
    public int _DAMAGE_REDUCTION; // cheat with this now

    public void UpdateHP(int newValue, int maxValue) {
        _CURRENT_HP = Mathf.Max(0, newValue);
        UpdateHPEvent(newValue, maxValue);
    }
}