using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

// All global enum definitions here
public static class Utils
{
	public static IEnumerator DelayedExecute(float delay, Action VoidAction) {
		yield return new WaitForSeconds(delay);
		VoidAction();
	}

	public static IEnumerator DelayedExecuteTicks(int delayTicks, Action VoidAction) {
		int count = 0;
		while (count < delayTicks) {
			count++;
			yield return null;
		}
		VoidAction();
	}
	
	public static IEnumerator DelayedFlag(bool flag, Action VoidAction) {
		while (flag) {
			yield return null;
		}
		VoidAction();
	}
}
