using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;
using Extensions;

public class NoiseMap
{
    public float[,,] octaves;
    private float[,] map;

    public float[,] Map { get => map; }

    public int width { get => map.GetLength(0); }
    public int height { get => map.GetLength(1); }


    public NoiseMap(int numOctaves, int xDim, int yDim) {
        octaves = new float[numOctaves, xDim, yDim];
        map = new float[xDim, yDim];
    }

    public float At(int x, int y) {
        return map[x, y];
    }

    public float[,] GetOctave(int octave) {
        float[,] retVal = new float[width, height];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                retVal[x, y] = octaves[octave, x, y];
            }
        }
        return retVal;
    }
    
    public void CombineOctaves() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                for (int o = 0; o < octaves.GetLength(0); o++) {
                    map[x, y] += octaves[o, x, y];
                }
            }
        }
    }
    
    // get the 0th octave, normalize differently, return
    public float[,] GetRidges() {
        return GetOctave(0).ApplyTransform(it => 1f - Mathf.Abs(Mathf.Lerp(-1f, 1f, it) ));
    }

    // fluent
    public NoiseMap Add(float[,] toAdd) {
        map = map.Add(toAdd);
        return this;
    }

    // fluent
    public NoiseMap Subtract(float[,] toSub) {
        map = map.Subtract(toSub);
        return this;
    }

    public void NormalizeMap() {
        map = map.Normalize();
    }
}