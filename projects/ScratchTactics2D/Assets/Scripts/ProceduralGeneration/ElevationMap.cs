using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;
using Extensions;
using MapTools;

public class ElevationMap : IPathable
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

    // fluent
    public ElevationMap Mask(int[,] toMask) {
        map = map.Mask<float>(toMask);
        return this;
    }

    public void NormalizeMap() {
        map = map.Normalize();
    }

    // IPathable definitions
    public IEnumerable<Vector3Int> GetNeighbors(Vector3Int origin) {
        Vector3Int up = origin + Vector3Int.up;
        Vector3Int right = origin + Vector3Int.right;
        Vector3Int down = origin + Vector3Int.down;
        Vector3Int left = origin + Vector3Int.left;
        if (map.Contains<float>(up.x, up.y)) yield return up;
        if (map.Contains<float>(right.x, right.y)) yield return right;
        if (map.Contains<float>(down.x, down.y)) yield return down;
        if (map.Contains<float>(left.x, left.y)) yield return left;
    }

    public int EdgeCost(Vector3Int src, Vector3Int dest) {
        return (int)(100f * (Mathf.Abs(map[dest.x, dest.y] - map[src.x, src.y]))) + 1;
    }
}