using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public interface IImmediateStatus
{
	// ie, Charge Bonus from ChargeUC
	bool revertWithMovement { get; set; }
}