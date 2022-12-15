using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public struct DamageContext
{
    private Dictionary<int, float> projection;
    public Dictionary<int, float> Projection => projection;
    public int Min => projection.Keys.Min();
    public int Max => projection.Keys.Max();

    public Func<int> Resolver;


    public DamageContext(Dictionary<int, float> damageProjection, Func<int> _DamageResolver) {
        projection = damageProjection;
        Resolver = _DamageResolver;
    }

    public DamageContext(int fixedDamage, Func<int> _DamageResolver) {
        projection = new Dictionary<int, float>(){
            [fixedDamage] = 1f
        };
        Resolver = _DamageResolver;
    }
}