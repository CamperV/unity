using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IPhasedObject
{
	bool phaseActionTaken { get; set; }

	bool MyPhase();
	bool TakePhaseAction();
}
