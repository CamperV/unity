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
    public event StatChange UpdateLuckEvent;
    public event StatChange UpdateMultistrikeEvent;

    private Unit boundUnit;

	public enum UpdateableStat {
		Vitality,
		Strength,
		Dexterity,
		Defense,
		Move,
		Multistrike
	}

    [HideInInspector] public int VITALITY;
    [HideInInspector] public int STRENGTH;
    [HideInInspector] public int DEXTERITY;
    [HideInInspector] public int REFLEX;
    [HideInInspector] public int DEFENSE;
    [HideInInspector] public int MOVE;
    [HideInInspector] public int _LUCK; // generally hidden. Useful in perks
    [HideInInspector] public int _MULTISTRIKE; // used in Engagement.Process()

    [HideInInspector] public int _CURRENT_HP;

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
    [SerializeField] private readonly int MAX_STAT_VALUE = 99;

    void Awake() {
        VITALITY  = baseStats.VITALITY  + Random.Range(-variance, variance);
        STRENGTH  = baseStats.STRENGTH  + Random.Range(-variance, variance);
        DEXTERITY = baseStats.DEXTERITY + Random.Range(-variance, variance);
        REFLEX    = baseStats.REFLEX    + Random.Range(-variance, variance);
        DEFENSE   = baseStats.DEFENSE   + Random.Range(-variance, variance);
        MOVE      = baseStats.MOVE;
        //
        _LUCK = 0;
        _MULTISTRIKE = 0;

        boundUnit = GetComponent<Unit>();
    }

    public int Calculate_ATK() {
        return STRENGTH + boundUnit.equippedWeapon.weaponStats.MIGHT;
    }

    public int Calculate_HIT() {
        return DEXTERITY*2 + boundUnit.equippedWeapon.weaponStats.ACCURACY;
    }

    public int Calculate_CRT() {
        return boundUnit.equippedWeapon.weaponStats.CRITICAL;
    }

    public int Calculate_AVO() {
        return REFLEX*2 - Mathf.Max(0, (boundUnit.equippedWeapon.weaponStats.WEIGHT - STRENGTH));
    }

    public int Calculate_CRTAVO() {
        // return (_LUCK + REFLEX) - Mathf.Max(0, (boundUnit.equippedWeapon.weaponStats.WEIGHT - STRENGTH));
        return _LUCK + REFLEX;
    }

    public Pair<int, int> CalculateDamageRange() {
        int upper = Mathf.Clamp(STRENGTH + boundUnit.equippedWeapon.weaponStats.MAX_MIGHT, 0, MAX_STAT_VALUE);
        int lower = Mathf.Clamp(DEXTERITY + boundUnit.equippedWeapon.weaponStats.MIN_MIGHT, 0, upper);
        return new Pair<int, int>(lower, upper);
    }

    // grant 5% extra crit for every point of lowerDmg over upperDmg
    public int CalculateCritical() {
        int upper = STRENGTH + boundUnit.equippedWeapon.weaponStats.MAX_MIGHT;
        int lower = DEXTERITY + boundUnit.equippedWeapon.weaponStats.MIN_MIGHT;
        int dexOverage = Mathf.Max(0, lower - upper);
        return boundUnit.equippedWeapon.weaponStats.CRITICAL + dexOverage*5;
    }

    public void ApplyNature(CampaignUnitGenerator.NatureData natureStats) {
        VITALITY  += natureStats.m_VITALITY;
        STRENGTH  += natureStats.m_STRENGTH;
        DEXTERITY += natureStats.m_DEXTERITY;
        REFLEX    += natureStats.m_REFLEX;
        DEFENSE   += natureStats.m_DEFENSE;
        MOVE      += natureStats.m_MOVE;
    }

    public void UpdateHP(int newValue, int maxValue) {
        _CURRENT_HP = Mathf.Clamp(newValue, 0, maxValue);
        UpdateHPEvent?.Invoke(_CURRENT_HP, maxValue);
    }

    public void UpdateVitality(int newValue) {
        VITALITY = Mathf.Clamp(newValue, 0, MAX_STAT_VALUE);
        UpdateVitalityEvent?.Invoke(newValue);
    }

    public void UpdateStrength(int newValue) {
        STRENGTH = Mathf.Clamp(newValue, 0, MAX_STAT_VALUE);
        UpdateStrengthEvent?.Invoke(newValue);
    }

    public void UpdateDexterity(int newValue) {
        DEXTERITY = Mathf.Clamp(newValue, 0, MAX_STAT_VALUE);
        UpdateDexterityEvent?.Invoke(newValue);
    }

    public void UpdateReflex(int newValue) {
        REFLEX = Mathf.Clamp(newValue, 0, MAX_STAT_VALUE);
        UpdateReflexEvent?.Invoke(newValue);
    }

    public void UpdateDefense(int newValue) {
        DEFENSE = Mathf.Clamp(newValue, 0, MAX_STAT_VALUE);
        UpdateDefenseEvent?.Invoke(newValue);
    }

    public void UpdateMove(int newValue) {
        MOVE = Mathf.Clamp(newValue, 0, MAX_STAT_VALUE);
        UpdateMoveEvent?.Invoke(newValue);
    }

    public void UpdateLuck(int newValue) {
        _LUCK = Mathf.Clamp(newValue, 0, MAX_STAT_VALUE);
        UpdateLuckEvent?.Invoke(newValue);
    }

    public void UpdateMultistrike(int newValue) {
        _MULTISTRIKE = Mathf.Clamp(newValue, 0, MAX_STAT_VALUE);
        UpdateMultistrikeEvent?.Invoke(newValue);
    }

    public void ModifyStat(UpdateableStat targetStat, int modifier) {
        switch (targetStat) {
			case UpdateableStat.Vitality:
				UpdateVitality(VITALITY + modifier);
				break;				
			case UpdateableStat.Strength:
				UpdateStrength(STRENGTH + modifier);
				break;	
			case UpdateableStat.Dexterity:
				UpdateDexterity(DEXTERITY + modifier);
				break;	
			case UpdateableStat.Defense:
                UpdateDefense(DEFENSE + modifier);
				break;	
			case UpdateableStat.Move:
				UpdateMove(MOVE + modifier);
				break;	
			case UpdateableStat.Multistrike:
				UpdateMultistrike(_MULTISTRIKE + modifier);
				break;	
		}
    }
}