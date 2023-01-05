using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class so_Status : ScriptableObject, IMutatorComponent
{
	public static string CreateStatusProviderID(IGUID provider, so_Status status) {
		return $"{status.name}@{provider.GUID.ToString()}";
	}

	public enum StatusCode {
		Buff,
		Debuff
	}
	public StatusCode statusCode;

	// other valuable data
	public bool stackable;

    // IMutatorComponent
    [field: SerializeField] public MutatorDisplayData mutatorDisplayData { get; set; }
	
    // assign this in the inspector
	public Sprite sprite;

    public virtual void OnAcquire(Unit thisUnit) {
		thisUnit.OnAttack += DisplayOnAttack;
    }

    public virtual void OnExpire(Unit thisUnit) {
		thisUnit.OnAttack -= DisplayOnAttack;
	}

	private void DisplayOnAttack(Unit thisUnit, ref MutableAttack mutAtt, Unit target) {
        mutAtt.AddMutator(this);
	}
}