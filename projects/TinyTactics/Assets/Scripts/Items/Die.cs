using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Misc/Die")]
public class Die : ScriptableObject
{
    public int maxFaceValue;    // ie D20 -> 20

    public int Min => 1;
    public int Max => maxFaceValue;

    public IEnumerable<int> Faces => Enumerable.Range(Min, Max);
    // public float ExpectedValue => Faces.Sum()/Faces.Count;
    public float BaseProbability => 1f/(float)maxFaceValue;

    public string Name => $"d{maxFaceValue}";

    public int Roll() {
        return Random.Range(Min, Max+1);
    }
}