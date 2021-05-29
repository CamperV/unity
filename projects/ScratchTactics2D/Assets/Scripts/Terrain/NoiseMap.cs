using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;
using Extensions;
using MapTools;

public class NoiseMap : ElevationMap
{
    public float[,,] octaves;

    public NoiseMap(int xDim, int yDim, int numOctaves) {
        octaves = new float[numOctaves, xDim, yDim];
        map = new float[xDim, yDim];
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
        return GetOctave(0).Map<float>(it => 1f - Mathf.Abs(Mathf.Lerp(-1f, 1f, it) ));
    }
}