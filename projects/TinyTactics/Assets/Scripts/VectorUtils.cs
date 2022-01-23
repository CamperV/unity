using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class VectorUtils
{
	public static Vector3 Centroid(params Vector3[] vectors) {
		Vector3 sum = Vector3.zero;
		foreach (var v in vectors) {
			sum += v;
		}
		return sum / vectors.Length;
	}
}
