using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Utils
{
	public static IEnumerator Delay(float delay) {
		yield return new WaitForSeconds(delay);
	}
	
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

	public static IEnumerator LateFrame(Action VoidAction) {
		yield return new WaitForEndOfFrame();
		VoidAction();
	}

	public static IEnumerator QueueCoroutines(params IEnumerator[] coroutines) {
		foreach (IEnumerator crt in coroutines) {
			yield return crt;
		}
	}
}

public class Pair<T, U> {
    public T First { get; set; }
    public U Second { get; set; }

    public Pair() {}
    public Pair(T first, U second) {
        this.First = first;
        this.Second = second;
    }
};
