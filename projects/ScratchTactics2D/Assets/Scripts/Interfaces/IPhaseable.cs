using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

interface IPhaseable
{
	float phaseDelayTime { get; }
	Enum.PhaseActionState phaseActionState { get; }
	void RegisterTo(TurnManager tm);
	void TriggerPhase();
	void EndPhase();
}