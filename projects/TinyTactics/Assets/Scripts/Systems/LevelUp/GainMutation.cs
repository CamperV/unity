using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "UnitData/GainMutation")]
public class GainMutation : LevelUp
{
    public Mutation mutation;
    
    public override void Apply(Unit thisUnit) {
        thisUnit.mutationSystem.AddMutation(mutation);
    }
}