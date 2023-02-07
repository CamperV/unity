using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Extensions;

[RequireComponent(typeof(UnitBroadcastEventSystem))]
public class ExperienceSystem : MonoBehaviour
{
    private Unit boundUnit;
    private UnitBroadcastEventSystem unitBroadcastEventSystem;

    // for binding UI, etc
    public delegate void ExperienceEvent(Unit thisUnit, int newValue);
    public event ExperienceEvent UpdateExperienceEvent;
    
    void Awake() {
        boundUnit = GetComponent<Unit>();
        unitBroadcastEventSystem = GetComponent<UnitBroadcastEventSystem>();
    }

    public void Initialize() {
        EXPERIENCE = 0;
        LEVEL = 0;

        boundUnit.OnDefeatTarget += DefaultExperienceGain;
    }

    public int EXPERIENCE;
    public int LEVEL;

    // use SO for these so that its consistent but I can debug different scaling
    public LevelingScale levelingScale;

    public void GainExperience(int experience) {
        EXPERIENCE += experience;
        UpdateExperienceEvent?.Invoke(boundUnit, EXPERIENCE);

        // check level thresholds for experience
        int level = levelingScale.Match(EXPERIENCE);
          
        // broadcast the level
        for (int lvl = LEVEL; lvl < level; lvl++) {         
            unitBroadcastEventSystem.OnLevelUp?.Invoke(boundUnit);
        }

        LEVEL = level;
    }

    private void DefaultExperienceGain(Unit thisUnit, Unit targetUnit) {
        GainExperience( (targetUnit as EnemyUnit).experienceReward );
    }
}