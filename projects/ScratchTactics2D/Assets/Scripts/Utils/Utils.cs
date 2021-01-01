﻿using System.Collections;
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
	
	public static List<T> RandomSelections<T>(List<T> list, int numSelections) {
		List<T> retVal = new List<T>();
		List<int> available = Enumerable.Range(0, list.Count).ToList();
		
		while (retVal.Count <= numSelections) {
			var avaIndex = Random.Range(0, available.Count);
			var selIndex = available[avaIndex];
			
			retVal.Add(list[selIndex]);
			available.Remove(selIndex);
		}
		return retVal;
	}

	public static Color selectColorBlue = new Color(0f, .75f, 1f, 1f);
	public static Color selectColorRed  = new Color(1f, .75f, 0f, 1f);
	public static Color threatColorRed  = new Color(1f, 0.50f, 0.50f, 1f);
}
