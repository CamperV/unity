using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class Mutation : ScriptableObject, IMutatorComponent
{
    // IMutatorComponent
    [field: SerializeField] public MutatorDisplayData mutatorDisplayData { get; set; }
	public Sprite sprite;
	
	public MutationArchetype archetype;

    public abstract void OnAcquire(Unit thisUnit);
    public abstract void OnRemove(Unit thisUnit);
}