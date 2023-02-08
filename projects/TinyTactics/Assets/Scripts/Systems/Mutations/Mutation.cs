using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class Mutation : ScriptableObject, IMutatorComponent
{
    // IMutatorComponent
    // .name
    // .description
    [field: SerializeField] public MutatorDisplayData mutatorDisplayData { get; set; }
	public Sprite sprite;
	
	public MutationArchetype archetype;

    // hidden statuses are granted by mutations and the like
	// they are supposed to be "internal" to the weapon, so don't always show them as a buff
	public bool hidden;

    public abstract void OnAcquire(Unit thisUnit);
    public abstract void OnRemove(Unit thisUnit);
}