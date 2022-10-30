using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class so_Status : ScriptableObject, IMutatorComponent
{
	public static string CreateStatusProviderID(IGUID provider, so_Status status) {
		return $"{status.name}@{provider.GUID.ToString()}";
	}

    // assign this in the inspector
    public new string name;
	public Sprite sprite;

	// other valuable data
	public bool stackable;

    // IMutatorComponent
	public string displayName {
		get => name;
		set => name = value;
	}

    public virtual void OnAcquire(Unit thisUnit) {
		thisUnit.OnAttack += DisplayModifiedAttack;
    }

    public virtual void OnExpire(Unit thisUnit) {
		thisUnit.OnAttack -= DisplayModifiedAttack;
	}

	private void DisplayModifiedAttack(Unit thisUnit, ref MutableAttack mutAtt, Unit target) {
        mutAtt.AddMutator(this);
	}
}