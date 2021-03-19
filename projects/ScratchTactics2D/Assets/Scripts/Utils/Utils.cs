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

	public static Color selectColorWhite  = new Color(1.00f, 1.00f, 1.00f, 0.45f);
	public static Color selectColorBlue   = new Color(0.00f, 0.30f, 0.75f, 0.75f);
	public static Color threatColorRed    = new Color(0.75f, 0.20f, 0.20f, 0.75f);
	public static Color threatColorYellow = new Color(0.75f, 0.75f, 0.00f, 0.75f);
	public static Color threatColorIndigo = new Color(0.30f, 0.30f, 0.75f, 0.75f);
}
