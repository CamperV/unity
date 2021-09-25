using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public interface IEnemyUnitClass
{
	// need to have a decision maker, such as who is best to attack and how to get there
	string assignedBrain { get; }
}