using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct Damage
{
    // Uniform Variance type
    // if there are other types, we can use an enum here, or subclass

    public int min;
    public int max;

    public int Mean => (int)((min + max) / 2f);

    public Damage(int _min, int _max) {
        min = _min;
        max = _max;
    }

    public Damage(int value) {
        min = value;
        max = value;
    }

    public Damage(Pair<int, int> damageRange) {
        min = damageRange.First;
        max = damageRange.Second;
    }

    public void Add(int value) {
        min += value;
        max += value;
    }

    public override string ToString() {
        if (min == max) {
            return $"{min}";
        } else {
            return $"{min} - {max}";
        }
    }

    public int Resolve() {
        return Random.Range(min, max+1);
    }
}