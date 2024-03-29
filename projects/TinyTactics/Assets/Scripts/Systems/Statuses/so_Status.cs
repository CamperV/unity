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
	
	// hidden statuses are granted by weapons and the like
	// they are supposed to be "internal" to the weapon, so don't always show them as a buff
	public bool hidden;	

    // IMutatorComponent
    [field: SerializeField] public MutatorDisplayData mutatorDisplayData { get; set; }
	
    // assign this in the inspector
	public Sprite sprite;
	public string emitMessage = "";

    public virtual void OnAcquire(Unit thisUnit) {
		thisUnit.OnAttackGeneration += DisplayOnAttack;
    }

    public virtual void OnExpire(Unit thisUnit) {
		thisUnit.OnAttackGeneration -= DisplayOnAttack;
	}

	private void DisplayOnAttack(Unit thisUnit, ref MutableAttack mutAtt, Unit target) {
        mutAtt.AddAttackMutator(this);
	}
}