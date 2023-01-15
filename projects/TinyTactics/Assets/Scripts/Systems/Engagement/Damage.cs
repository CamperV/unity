using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct Damage
{
    public enum DamageType {
        Normal,
        Poise
    };
    public DamageType damageType;

    public int min;
    public int max;

    public int Mean => (int)((min + max) / 2f);

    public Damage(int _min, int _max, DamageType _damageType = DamageType.Normal) {
        min = _min;
        max = _max;
        damageType = _damageType;
    }

    public Damage(int value, DamageType _damageType = DamageType.Normal) {
        min = value;
        max = value;
        damageType = _damageType;
    }

    public Damage(Pair<int, int> damageRange, DamageType _damageType = DamageType.Normal) {
        min = damageRange.First;
        max = damageRange.Second;
        damageType = _damageType;
    }

    ///////////////
    // operators //
    ///////////////
    public bool Equals(Damage other) {
        return min == other.min && max == other.max && damageType == other.damageType;
    }

    public static bool operator ==(Damage d, Damage other) {
        return d.Equals(other);
    }

    public static bool operator !=(Damage d, Damage other) {
        return !d.Equals(other);
    }

    public static Damage operator+(Damage a, Damage b) {
        return new Damage(a.min + b.min, a.max + b.max);
    }

    public static Damage operator-(Damage a, Damage b) {
        return new Damage(a.min - b.min, a.max - b.max);
    }

    public static Damage operator*(Damage a, Damage b) {
        return new Damage(a.min * b.min, a.max * b.max);
    }

    public static Damage operator-(Damage a) {
        return new Damage(-a.min, -a.max);
    }

    public static Damage operator*(Damage a, int b) {
        return new Damage(a.min * b, a.max * b);
    }

    public static Damage operator*(int a, Damage b) {
        return new Damage(a * b.min, a * b.max);
    }

    public static Damage operator/(Damage a, int b) {
        return new Damage(a.min / b, a.max / b);
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