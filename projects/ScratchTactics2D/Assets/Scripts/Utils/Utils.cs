using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

// All global enum definitions here
public static class Utils
{
	public static IEnumerator DelayedExecute(float delay, Action voidAction) {
		yield return new WaitForSeconds(delay);
		voidAction();
	}
	
	public static List<T> RandomSelections<T>(List<T> list, int numSelections) {
		List<T> retVal = new List<T>();
		List<int> available = Enumerable.Range(0, list.Count).ToList();
		
		while (retVal.Count <= numSelections) {
			var avaIndex = Random.Range(0, available.Count);
			var selIndex = available[avaIndex];
			
			retVal.Add(list[selIndex]);
			available.Remove(avaIndex);
		}
		return retVal;
	}
}
