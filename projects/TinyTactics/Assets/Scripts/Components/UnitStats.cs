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
    public event StatChange UpdateVitalityEvent;
    public event StatChange UpdateStrengthEvent;
    public event StatChange UpdateDexterityEvent;
    public event StatChange UpdateReflexEvent;
    public event StatChange UpdateDefenseEvent;
    public event StatChange UpdateMoveEvent;

    private Unit boundUnit;

    [HideInInspector] public int VITALITY;
    [HideInInspector] public int STRENGTH;
    [HideInInspector] public int DEXTERITY;
    [HideInInspector] public int REFLEX;
    [HideInInspector] public int DEFENSE;
    [HideInInspector] public int MOVE;

    [HideInInspector] public int _CURRENT_HP;
    [HideInInspector] public int _ATK => STRENGTH + boundUnit.equippedWeapon.weaponStats.MIGHT;
    [HideInInspector] public int _HIT => DEXTERITY*2 + boundUnit.equippedWeapon.weaponStats.ACCURACY;
    [HideInInspector] public int _AVO => REFLEX*2 - Mathf.Max(0, (boundUnit.equippedWeapon.weaponStats.WEIGHT - STRENGTH));

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
    [SerializeField] private int variance = 2;

    void Awake() {
        VITALITY  = baseStats.VITALITY  + Random.Range(-variance, variance);
        STRENGTH  = baseStats.STRENGTH  + Random.Range(-variance, variance);
        DEXTERITY = baseStats.DEXTERITY + Random.Range(-variance, variance);
        REFLEX    = baseStats.REFLEX    + Random.Range(-variance, variance);
        DEFENSE   = baseStats.DEFENSE   + Random.Range(-variance, variance);
        MOVE      = baseStats.MOVE;

        boundUnit = GetComponent<Unit>();
    }

    public void UpdateHP(int newValue, int maxValue) {
        _CURRENT_HP = Mathf.Clamp(newValue, 0, maxValue);
        UpdateHPEvent?.Invoke(_CURRENT_HP, maxValue);
    }

    public void UpdateStrength(int newValue) {
        STRENGTH = newValue;
        UpdateStrengthEvent?.Invoke(newValue);
    }

    public void UpdateReflex(int newValue) {
        REFLEX = newValue;
        UpdateReflexEvent?.Invoke(newValue);
    }

    public void UpdateDefense(int newValue) {
        DEFENSE = newValue;
        UpdateDefenseEvent?.Invoke(newValue);
    }
}