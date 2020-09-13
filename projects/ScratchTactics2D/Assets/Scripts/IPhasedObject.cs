using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IPhasedObject
{
	bool phaseActionTaken { get; }
	bool MyPhase();
	void TriggerPhase();
	bool TakePhaseAction();
}
