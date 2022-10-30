using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class Mutation : ScriptableObject, IMutatorComponent
{
    // assign this in the inspector
    public new string name;
	public string description;
	public Sprite sprite;

    // IMutatorComponent
	public string displayName {
		get => name;
		set => name = value;
	}
	
	public MutationArchetype archetype;

    public abstract void OnAcquire(Unit thisUnit);
    public abstract void OnRemove(Unit thisUnit);
}