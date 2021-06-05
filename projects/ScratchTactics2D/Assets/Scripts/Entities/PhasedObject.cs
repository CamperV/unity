using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

interface IPhasedObject
{
	Enum.PhaseActionState phaseActionState { get; }
	Enum.Phase myPhase { get; }
	bool MyPhaseActive();
	void TriggerPhase();
}

public abstract class PhasedObject : MonoBehaviour, IPhasedObject
{
	public const float phaseDelayTime = 0.01f;	// in units of WaitForSeconds();
	[HideInInspector] public Enum.PhaseActionState phaseActionState { get; protected set; }
	[HideInInspector] public Enum.Phase myPhase { get; protected set; }
	
	void Start() {
		phaseActionState = Enum.PhaseActionState.waitingForInput;
	}
	
	public virtual void TriggerPhase() {
		phaseActionState = Enum.PhaseActionState.waitingForInput;
	}
	
	public virtual void EndPhase(float timeOverride = phaseDelayTime) {
		// then reset your phase, and mark as complete
		StartCoroutine(Utils.DelayedExecute(timeOverride, () => {
			phaseActionState = Enum.PhaseActionState.postPhase;
		})); 
	}
	
	// only for overworld objects, right now
	public virtual bool MyPhaseActive() {
		return GameManager.inst.phaseManager.currentPhase == myPhase;
	}
}