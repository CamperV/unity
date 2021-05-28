using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;
using Extensions;
using MapTools;

public class ElevationMap
{
    protected float[,] map;
    public float[,] Map { get => map; }

    public int width { get => map.GetLength(0); }
    public int height { get => map.GetLength(1); }

    public ElevationMap() {}
    public ElevationMap(int xDim, int yDim) {
        map = new float[xDim, yDim];
    }

    public float At(int x, int y) {
        return map[x, y];
    }

    // fluent
    public ElevationMap Add(float[,] toAdd) {
        map = map.Add(toAdd);
        return this;
    }

    // fluent
    public ElevationMap Subtract(float[,] toSub) {
        map = map.Subtract(toSub);
        return this;
    }

    public void NormalizeMap() {
        map = map.Normalize();
    }
}