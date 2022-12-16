using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class statSystem : MonoBehaviour
{
    // publicly visible events for UIs etc to key off of
	public delegate void StatRatioChange(int newValue, int maxValue);
    public event StatRatioChange UpdateHPEvent;
    public event StatRatioChange UpdateBreakEvent;

    public delegate void StatChange(int newValue);
    public event StatChange UpdateVitalityEvent;
    public event StatChange UpdateStrengthEvent;
    public event StatChange UpdateDexterityEvent;
    public event StatChange UpdateBrawnEvent;
    public event StatChange UpdateFinesseEvent;
    public event StatChange UpdateReflexEvent;
    public event StatChange UpdateDefenseEvent;
    public event StatChange UpdateMoveEvent;
    public event StatChange UpdateLuckEvent;
    public event StatChange UpdateMultistrikeEvent;

	public enum UpdatableStat {
		Vitality,
        Brawn,
        Finesse,
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

    public int BRAWN;
    public int FINESSE;

    [HideInInspector] public int _CURRENT_HP;
    [HideInInspector] public int _CURRENT_BREAK;

    [Serializable]
    public struct BaseStats {
        public int VITALITY;
        //
        public int BRAWN;
        public int FINESSE;
        //
        // public int STRENGTH;
        // public int DEXTERITY;
        // public int REFLEX;
        public int DEFENSE;
        public int MOVE;
        //
        public int _MULTISTRIKE;

    }
    [SerializeField] private BaseStats baseStats;
    [SerializeField] private int variance = 0;
    public static int _MAX_STAT_VALUE = 99;

    void Awake() {
        VITALITY  = baseStats.VITALITY  + Random.Range(-variance, variance);
        //
        BRAWN     = baseStats.BRAWN     + Random.Range(-variance, variance);
        FINESSE   = baseStats.FINESSE   + Random.Range(-variance, variance);
        //
        // STRENGTH  = baseStats.STRENGTH  + Random.Range(-variance, variance);
        // DEXTERITY = baseStats.DEXTERITY + Random.Range(-variance, variance);
        // REFLEX    = baseStats.REFLEX    + Random.Range(-variance, variance);
        STRENGTH  = 0                   + Random.Range(-variance, variance);
        DEXTERITY = 0                   + Random.Range(-variance, variance);
        REFLEX    = 0                   + Random.Range(-variance, variance);
        DEFENSE   = baseStats.DEFENSE   + Random.Range(-variance, variance);
        MOVE      = baseStats.MOVE;
        //
        _LUCK = 0;
        _MULTISTRIKE = baseStats._MULTISTRIKE;
    }

    public void ApplyNature(CampaignUnitGenerator.NatureData natureStats) {
        // VITALITY  += natureStats.m_VITALITY;
        // STRENGTH  += natureStats.m_STRENGTH;
        // DEXTERITY += natureStats.m_DEXTERITY;
        // REFLEX    += natureStats.m_REFLEX;
        // DEFENSE   += natureStats.m_DEFENSE;
        // MOVE      += natureStats.m_MOVE;
    }

    public int MissingHP() {
        return VITALITY - _CURRENT_HP;
    }

    public void UpdateHP(int newValue, int maxValue) {
        _CURRENT_HP = Mathf.Clamp(newValue, 0, maxValue);
        UpdateHPEvent?.Invoke(_CURRENT_HP, maxValue);
    }

    public void UpdateVitality(int newValue) {
        VITALITY = Mathf.Clamp(newValue, 0, _MAX_STAT_VALUE);
        UpdateVitalityEvent?.Invoke(newValue);
    }

    public void UpdateStrength(int newValue) {
        STRENGTH = Mathf.Clamp(newValue, 0, _MAX_STAT_VALUE);
        UpdateStrengthEvent?.Invoke(newValue);
    }

    public void UpdateDexterity(int newValue) {
        DEXTERITY = Mathf.Clamp(newValue, 0, _MAX_STAT_VALUE);
        UpdateDexterityEvent?.Invoke(newValue);
    }

    public void UpdateBreak(int newValue, int maxValue) {
        _CURRENT_BREAK = Mathf.Clamp(newValue, 0, maxValue);
        UpdateBreakEvent?.Invoke(_CURRENT_BREAK, maxValue);
    }

    public void UpdateBrawn(int newValue) {
        BRAWN = Mathf.Clamp(newValue, 0, _MAX_STAT_VALUE);
        UpdateBrawnEvent?.Invoke(newValue);
        UpdateBreak(_CURRENT_BREAK, newValue);
    }

    public void UpdateFinesse(int newValue) {
        FINESSE = Mathf.Clamp(newValue, 0, _MAX_STAT_VALUE);
        UpdateFinesseEvent?.Invoke(newValue);
    }

    public void UpdateReflex(int newValue) {
        REFLEX = Mathf.Clamp(newValue, 0, _MAX_STAT_VALUE);
        UpdateReflexEvent?.Invoke(newValue);
    }

    public void UpdateDefense(int newValue) {
        DEFENSE = Mathf.Clamp(newValue, 0, _MAX_STAT_VALUE);
        UpdateDefenseEvent?.Invoke(newValue);
    }

    public void UpdateMove(int newValue) {
        MOVE = Mathf.Clamp(newValue, 0, _MAX_STAT_VALUE);
        UpdateMoveEvent?.Invoke(newValue);
    }

    public void UpdateLuck(int newValue) {
        _LUCK = Mathf.Clamp(newValue, 0, _MAX_STAT_VALUE);
        UpdateLuckEvent?.Invoke(newValue);
    }

    public void UpdateMultistrike(int newValue) {
        _MULTISTRIKE = Mathf.Clamp(newValue, 0, _MAX_STAT_VALUE);
        UpdateMultistrikeEvent?.Invoke(newValue);
    }

    public void ModifyStat(UpdatableStat targetStat, int modifier) {
        switch (targetStat) {
			case UpdatableStat.Vitality:
				UpdateVitality(VITALITY + modifier);
				break;			
            case UpdatableStat.Brawn:
				UpdateBrawn(BRAWN + modifier);
				break;	
			case UpdatableStat.Finesse:
				UpdateFinesse(FINESSE + modifier);
				break;		
			case UpdatableStat.Strength:
				UpdateStrength(STRENGTH + modifier);
				break;	
			case UpdatableStat.Dexterity:
				UpdateDexterity(DEXTERITY + modifier);
				break;	
			case UpdatableStat.Defense:
                UpdateDefense(DEFENSE + modifier);
				break;	
			case UpdatableStat.Move:
				UpdateMove(MOVE + modifier);
				break;	
			case UpdatableStat.Multistrike:
				UpdateMultistrike(_MULTISTRIKE + modifier);
				break;	
		}
    }
}