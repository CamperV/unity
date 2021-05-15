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

public class PerlinTerrainGenerator : ElevationTerrainGenerator
{
    [Header("Perlin Noise Settings")]
	public int seed;
	public float scale;
	public int octaves;
    [Range(0, 4)]
    public float power;

	public override void GenerateMap() {
        map = new TileEnum[mapDimensionX, mapDimensionY];

        // Texture2D template = LoadTemplate("upwards");
        // SaveTextureAsPNG(template, "template.png");
        //float[,] noise = GenerateNoiseMap(additive: TextureAsFloat(template));
        
        float[,] beachMask = GenerateLogGradient(scale: 2.0f);
        SaveTextureAsPNG(RawTexture(beachMask), "beachMask.png");

        float[,] mountainMask = GenerateExpGradient(4.0f, reversed: true);
        SaveTextureAsPNG(RawTexture(mountainMask), "mountainMask.png");
        
        float[,] noise = GenerateNoiseMap().Add(beachMask).Add(mountainMask).Normalize();
        
        // save the noise as a Texture2D
        Texture2D rawTexture = RawTexture(noise);
        Texture2D terrainTexture = ColorizedTexture(noise);
        SaveTextureAsPNG(rawTexture, "noise_map.png");
        SaveTextureAsPNG(terrainTexture, "terrain_map.png");
    	
		for (int i = 0; i < map.GetLength(0); i++) {
			for (int j = 0; j < map.GetLength(1); j++) {
				map[i, j] = ElevationToTile(noise[i, j]);
			}
		}
    }

    private float[,] GenerateNoiseMap() {
        float[,] noise = new float[mapDimensionX, mapDimensionY];
        
        // When changing noise scale, it zooms from top-right corner
        // This will make it zoom from the center
        Vector2 midpoint = new Vector2(mapDimensionX/2f, mapDimensionY/2f);

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        if (seed != -1) Random.InitState(seed);
        Vector2 randomOffset = new Vector2(Random.Range(-10000, 10000), Random.Range(-10000, 10000));
    
        for (int x = 0; x < mapDimensionX; x++) {
            for (int y = 0; y < mapDimensionY; y++) {
                float noiseHeight = 0.0f;

                for (float o = 0f; o < octaves; o++) {         
                    float octaveScale = Mathf.Pow(2f, o);  // 1, 2, 4, 8, etc

                    float sampleX = (float)(x - midpoint.x)/scale;
                    float sampleY = (float)(y - midpoint.y)/scale;
                    Vector2 sample = octaveScale * new Vector2(sampleX, sampleY) + randomOffset;

                    float perlinValue = 1/octaveScale * Mathf.PerlinNoise(sample.x, sample.y);
                    noiseHeight += Mathf.Pow(perlinValue, power);
                }

                // assign temp non-lerped value here
                noise[x, y] = noiseHeight;

                // keep track of the bounds as we create them
                maxNoiseHeight = Mathf.Max(noiseHeight, maxNoiseHeight);
                minNoiseHeight = Mathf.Min(noiseHeight, minNoiseHeight);
            }
        }

        // go for a second pass and lerp the noise values between the max/min
        for (int x = 0; x < mapDimensionX; x++) {
            for (int y = 0; y < mapDimensionY; y++) {
                noise[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noise[x, y]);
            }
        }

        return noise;
    }

    public override void Postprocessing() {}
}