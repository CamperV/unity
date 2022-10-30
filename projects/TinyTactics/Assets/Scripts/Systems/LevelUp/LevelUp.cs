using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class LevelUp : ScriptableObject
{
    public abstract void Apply(Unit thisUnit);
}