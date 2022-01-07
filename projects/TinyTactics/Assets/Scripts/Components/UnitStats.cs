using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class UnitStats : MonoBehaviour
{
    // publicly visible events for UIs etc to key off of
	public delegate void StatRatioChange(int newValue, int maxValue);
    public event StatRatioChange UpdateHPEvent;

    public delegate void StatChange(int newValue);
    public event StatChange UpdateReflexEvent;
    public event StatChange UpdateStrengthEvent;

    public int VITALITY;
    public int STRENGTH;
    public int DEXTERITY;
    public int REFLEX;
    public int DEFENSE;
    public int MOVE;

    public int _CURRENT_HP;

    [Serializable]
    public struct BaseStats {
        public int VITALITY;
        public int STRENGTH;
        public int DEXTERITY;
        public int REFLEX;
        public int DEFENSE;
        public int MOVE;
    }
    [SerializeField] private BaseStats baseStats;
    [SerializeField] private int variance = 3;

    void Awake() {
        VITALITY  = baseStats.VITALITY  + Random.Range(-variance, variance);
        STRENGTH  = baseStats.STRENGTH  + Random.Range(-variance, variance);
        DEXTERITY = baseStats.DEXTERITY + Random.Range(-variance, variance);
        REFLEX    = baseStats.REFLEX    + Random.Range(-variance, variance);
        DEFENSE   = baseStats.DEFENSE   + Random.Range(-variance, variance);
        MOVE      = baseStats.MOVE;
    }

    public void UpdateHP(int newValue, int maxValue) {
        _CURRENT_HP = Mathf.Clamp(newValue, 0, maxValue);
        UpdateHPEvent?.Invoke(_CURRENT_HP, maxValue);
    }

    public void UpdateReflex(int newValue) {
        // REFLEX = Mathf.Clamp(newValue, 0, 99);
        REFLEX = newValue;
        UpdateReflexEvent?.Invoke(newValue);
    }

    public void UpdateStrength(int newValue) {
        STRENGTH = newValue;
        UpdateStrengthEvent?.Invoke(newValue);
    }
}