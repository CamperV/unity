using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class so_Status : ScriptableObject, IMutatorComponent
{
    // assign this in the inspector
    public new string name;
	public string description;
	public Sprite sprite;
	public bool stackable;

    // IMutatorComponent
	public string displayName {
		get => name;
		set => name = value;
	}

    public abstract void OnAcquire(Unit thisUnit);
    public abstract void OnExpire(Unit thisUnit);
}