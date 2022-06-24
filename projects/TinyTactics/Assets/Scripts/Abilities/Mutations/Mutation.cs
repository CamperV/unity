using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class Mutation : ScriptableObject
{
    // assign this in the inspector
    public new string name;
	public string typeName;
	public string description;
	public Sprite sprite;
	//
	public ArchetypeData belongsToArchetype;
	public bool isSignature;

    public abstract void OnAcquire(Unit thisUnit);
    public abstract void OnRemove(Unit thisUnit);
}