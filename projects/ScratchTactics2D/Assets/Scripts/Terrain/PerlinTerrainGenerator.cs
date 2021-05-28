using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Extensions;
using MapTools;

public class PerlinTerrainGenerator : ElevationTerrainGenerator
{
    [Header("Perlin Noise Settings")]
	public int seed;
	public float scale;
	public int octaves;
    [Range(0, 4)]
    public float power;

	public override void GenerateMap() {        
        // make sure the bottom part of the map is beachy
        float[,] beachMask = GenerateLogGradient(scale: 2.0f);
        SaveTextureAsPNG(RawTexture(beachMask), "beachMask.png");

        // make sure the top part of the map is mountainous
        float[,] mountainMask = GenerateExpGradient(4.0f, reversed: true);
        SaveTextureAsPNG(RawTexture(mountainMask), "mountainMask.png");

        // add dimples to add "natural" lakes
        float[,] lakeMask = GenerateRandomDimples(verticalThreshold: (int)(mapDimensionY/4f), seed: seed);
        SaveTextureAsPNG(RawTexture(lakeMask), "lakeMask.png");
        
        elevation = GenerateNoiseMap().Add(beachMask).Add(mountainMask).Subtract(lakeMask);
        elevation.NormalizeMap();
        
        // save the noise as a Texture2D
        Texture2D rawTexture = RawTexture(elevation.Map);
        Texture2D terrainTexture = ColorizedTexture(elevation.Map);
        SaveTextureAsPNG(rawTexture, "noise_map.png");
        SaveTextureAsPNG(terrainTexture, "terrain_map.png");
    	
        map = new TileEnum[mapDimensionX, mapDimensionY];
		for (int i = 0; i < map.GetLength(0); i++) {
			for (int j = 0; j < map.GetLength(1); j++) {
				map[i, j] = ElevationToTile( elevation.At(i, j) );
			}
		}
    }

    protected virtual NoiseMap GenerateNoiseMap() {
        NoiseMap noiseMap = new NoiseMap(octaves, mapDimensionX, mapDimensionY);
        
        // When changing noise scale, it zooms from top-right corner
        // This will make it zoom from the center
        Vector2 midpoint = new Vector2(mapDimensionX/2f, mapDimensionY/2f);

        if (seed != -1) Random.InitState(seed);
        Vector2 randomOffset = new Vector2(Random.Range(-10000, 10000), Random.Range(-10000, 10000));
    
        for (int x = 0; x < mapDimensionX; x++) {
            for (int y = 0; y < mapDimensionY; y++) {

                // fill out each octave x-y
                for (int o = 0; o < octaves; o++) {         
                    float octaveScale = Mathf.Pow(2f, o);  // 1, 2, 4, 8, etc

                    float sampleX = (float)(x - midpoint.x)/scale;
                    float sampleY = (float)(y - midpoint.y)/scale;
                    Vector2 sample = octaveScale * new Vector2(sampleX, sampleY) + randomOffset;

                    float perlinValue = Mathf.Pow( 1/octaveScale * Mathf.PerlinNoise(sample.x, sample.y), power);
                    noiseMap.octaves[o, x, y] = perlinValue;
                }
            }
        }
        
        noiseMap.CombineOctaves();
        noiseMap.NormalizeMap();
        return noiseMap;
    }
}