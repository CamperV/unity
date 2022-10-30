using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Extensions;

public class ExperienceSystem : MonoBehaviour
{
    private Unit boundUnit;

    // for binding UI, etc
    public delegate void ExperienceEvent(Unit thisUnit, int newValue);
    public event ExperienceEvent UpdateLevelEvent;
    public event ExperienceEvent UpdateExperienceEvent;
    
    void Awake() {
        boundUnit = GetComponent<Unit>();
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
    public LevelProgression levelProgression;

    public void GainExperience(int experience) {
        EXPERIENCE += experience;
        UpdateExperienceEvent?.Invoke(boundUnit, EXPERIENCE);

        // check level thresholds for experience
        int level = levelingScale.Match(EXPERIENCE);
        Debug.Log($"Starting at level {LEVEL}, found match {level} for {EXPERIENCE}");
        
        for (int lvl = LEVEL; lvl < level; lvl++) {
            Debug.Log($"{lvl} is providing {levelProgression.levelUpProgression[lvl]}");
            levelProgression.levelUpProgression[lvl].Apply(boundUnit);
            UpdateLevelEvent?.Invoke(boundUnit, lvl);
        }

        LEVEL = level;
    }

    private void DefaultExperienceGain(Unit thisUnit, Unit targetUnit) {
        GainExperience( (targetUnit as EnemyUnit).experienceReward );
    }
}