using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "UnitData/StatIncrease")]
public class StatIncrease : LevelUp
{
    public UnitStats.UpdatableStat stat;
    public int value;

    public override void Apply(Unit thisUnit) {
        Debug.Log($"{thisUnit} is gaining {value} in {stat}");
        thisUnit.unitStats.ModifyStat(stat, value);
    }
}