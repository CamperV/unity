using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// All global enum definitions here
public static class Utils
{
	public static IEnumerator DelayedExecute(float delay, Action voidAction) {
		yield return new WaitForSeconds(delay);
		voidAction();
	}
}
