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
	void TakePhaseAction();
}

public abstract class PhasedObject : MonoBehaviour, IPhasedObject
{
	public readonly float phaseDelayTime = 0.05f;	// in units of WaitForSeconds();
	[HideInInspector] public Enum.PhaseActionState phaseActionState { get; protected set; }
	[HideInInspector] public Enum.Phase myPhase { get; protected set; }
	
	void Start() {
		phaseActionState = Enum.PhaseActionState.waitingForInput;
	}
	
	public void TriggerPhase() {
		phaseActionState = Enum.PhaseActionState.waitingForInput;
	}
	
	public void EndPhase() {
		// then reset your phase, and mark as complete
		StartCoroutine(Utils.DelayedExecute(phaseDelayTime, () => {
			phaseActionState = Enum.PhaseActionState.postPhase;
		})); 
	}
	
	public bool MyPhaseActive() {
		return GameManager.inst.phaseManager.currentPhase == myPhase;
	}
	
	//
	public virtual void TakePhaseAction() {
		throw new System.InvalidOperationException("TakePhaseAction must be defined at a lower-level.");
	}
}