using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class TerrainEffect : ScriptableObject, IMutatorComponent
{
    public string displayName { get; set; }
    public abstract string shortDisplayName { get; set; }
    
    public abstract void OnEnterTerrain(Unit targetUnit);
    public abstract void OnExitTerrain(Unit targetUnit);
}