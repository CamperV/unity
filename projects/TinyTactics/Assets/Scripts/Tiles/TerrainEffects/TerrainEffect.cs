using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class TerrainEffect : ScriptableObject, IMutatorComponent
{
    // IMutatorComponent
    [field: SerializeField] public MutatorDisplayData mutatorDisplayData { get; set; }
    
    public abstract void OnEnterTerrain(Unit targetUnit);
    public abstract void OnExitTerrain(Unit targetUnit);
}