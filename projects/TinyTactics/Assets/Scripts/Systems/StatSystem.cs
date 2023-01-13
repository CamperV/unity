using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class StatSystem : MonoBehaviour
{
    // publicly visible events for UIs etc to key off of
	public delegate void StatRatioChange(int newValue, int maxValue);
    public event StatRatioChange UpdateHPEvent;
    public event StatRatioChange UpdatePoiseEvent;

    public delegate void StatChange(int newValue);
    public event StatChange UpdateDamageReductionEvent;
    public event StatChange UpdateMoveEvent;
    public event StatChange UpdateMultistrikeEvent;

	public enum UpdatableStat {
		CURRENT_HP,
        MAX_HP,
        CURRENT_POISE,
        MAX_POISE,
        DAMAGE_REDUCTION,
        MOVE,
        MULTISTRIKE
	}

    [Header("Current Stat Values")]
    public int CURRENT_HP;
    public int MAX_HP;
    public int CURRENT_POISE;
    public int MAX_POISE;
    public int DAMAGE_REDUCTION;
    public int MOVE;
    public int MULTISTRIKE;

    public int MissingHP => (MAX_HP - CURRENT_HP);
    public bool CounterAttackAvailable => (CURRENT_POISE > 0);

    [Serializable]
    public struct BaseStats {
        public int MAX_HP;
        public int MAX_POISE;
        public int DAMAGE_REDUCTION;
        public int MOVE;
        public int MULTISTRIKE;
    }
    [SerializeField] private BaseStats baseStats;
    public static int _MAX_STAT_VALUE = 99;

    void Awake() {
        CURRENT_HP = baseStats.MAX_HP;
        MAX_HP = baseStats.MAX_HP;
        CURRENT_POISE = baseStats.MAX_POISE;
        MAX_POISE = baseStats.MAX_POISE;
        DAMAGE_REDUCTION = baseStats.DAMAGE_REDUCTION;
        MOVE = baseStats.MOVE;
        MULTISTRIKE = baseStats.MULTISTRIKE;
    }

    public void Initialize() {
        UpdateHP(CURRENT_HP, MAX_HP);
        UpdatePoise(CURRENT_POISE, MAX_POISE);
        UpdateDamageReduction(DAMAGE_REDUCTION);
        UpdateMove(MOVE);
        UpdateMultistrike(MULTISTRIKE);
    }

    public void UpdateHP(int newValue, int maxValue) {
        CURRENT_HP = Mathf.Clamp(newValue, 0, maxValue);
        MAX_HP = Mathf.Clamp(maxValue, 0, _MAX_STAT_VALUE);
        UpdateHPEvent?.Invoke(CURRENT_HP, MAX_HP);       
    }

    public void UpdateHP(int newValue) {
        CURRENT_HP = Mathf.Clamp(newValue, 0, MAX_HP);
        UpdateHPEvent?.Invoke(CURRENT_HP, MAX_HP);       
    }

    public void UpdatePoise(int newValue, int maxValue) {
        CURRENT_POISE = Mathf.Clamp(newValue, 0, maxValue);
        MAX_POISE = Mathf.Clamp(maxValue, 0, _MAX_STAT_VALUE);
        UpdatePoiseEvent?.Invoke(CURRENT_POISE, MAX_POISE);
    }

    public void UpdatePoise(int newValue) {
        CURRENT_POISE = Mathf.Clamp(newValue, 0, MAX_POISE);
        UpdatePoiseEvent?.Invoke(CURRENT_POISE, MAX_POISE);
    }

    public void UpdateDamageReduction(int newValue) {
        DAMAGE_REDUCTION = Mathf.Clamp(newValue, 0, _MAX_STAT_VALUE);
        UpdateDamageReductionEvent?.Invoke(newValue);
    }

    public void UpdateMove(int newValue) {
        MOVE = Mathf.Clamp(newValue, 0, _MAX_STAT_VALUE);
        UpdateMoveEvent?.Invoke(newValue);
    }

    public void UpdateMultistrike(int newValue) {
        MULTISTRIKE = Mathf.Clamp(newValue, 0, _MAX_STAT_VALUE);
        UpdateMultistrikeEvent?.Invoke(newValue);
    }

    public void ModifyStat(UpdatableStat targetStat, int modifier) {
        switch (targetStat) {
            case UpdatableStat.CURRENT_HP:
                UpdateHP(CURRENT_HP + modifier, MAX_HP);
                break;
            case UpdatableStat.MAX_HP:
                UpdateHP(CURRENT_HP, MAX_HP + modifier);
                break;
            case UpdatableStat.CURRENT_POISE:
                UpdatePoise(CURRENT_POISE + modifier, MAX_POISE);
                break;
            case UpdatableStat.MAX_POISE:
                UpdatePoise(CURRENT_POISE, MAX_POISE + modifier);
                break;
			case UpdatableStat.DAMAGE_REDUCTION:
                UpdateDamageReduction(DAMAGE_REDUCTION + modifier);
				break;	
			case UpdatableStat.MOVE:
				UpdateMove(MOVE + modifier);
				break;	
			case UpdatableStat.MULTISTRIKE:
				UpdateMultistrike(MULTISTRIKE + modifier);
				break;	
		}
    }
}