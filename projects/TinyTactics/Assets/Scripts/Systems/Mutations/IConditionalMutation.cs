using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public interface IConditionalMutation
{
	bool ConditionMet(List<Unit> units);
}