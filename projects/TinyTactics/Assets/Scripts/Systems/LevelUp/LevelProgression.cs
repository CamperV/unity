using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "UnitData/LevelProgression")]
public class LevelProgression : ScriptableObject
{
    public LevelUp[] levelUpProgression;
}