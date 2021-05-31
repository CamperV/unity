﻿using System.Collections;
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
	public float perlinScale;
	public int octaves;
    [Range(0, 4)]
    public float perlinPower;

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
        
        elevation = GeneratePerlinNoiseMap(mapDimensionX, mapDimensionY, octaves, scale: perlinScale, power: perlinPower);
        elevation.Add(beachMask).Add(mountainMask).Subtract(lakeMask);
        elevation.NormalizeMap();
        
        // save the noise as a Texture2D
        SaveTextureAsPNG( RawTexture(elevation.Map), "noise_map.png");
        SaveTextureAsPNG( ColorizedTexture(elevation.Map), "terrain_map.png");
    	
        map = new TileEnum[mapDimensionX, mapDimensionY];
		for (int i = 0; i < map.GetLength(0); i++) {
			for (int j = 0; j < map.GetLength(1); j++) {
				map[i, j] = ElevationToTile( elevation.At(i, j) );
			}
		}
    }

    // Preprocessing is run before ApplyMap
    protected override void Preprocessing() {
        //
        // smooth beach?
        //

        // seed + grow forests
        // > generate a separate noise map, smoother, to generate forests
        // TODO: I really just kinda smooth-brained this one out: there's a better way to balance the multiple thresholds/rng here
        NoiseMap forestProbabilityMap = GeneratePerlinNoiseMap(mapDimensionX, mapDimensionY, 1, scale: perlinScale*2.0f, power: 0.75f);
        forestProbabilityMap.Mask( elevation.Map.ClampBinaryThreshold(0.45f, 0.85f) );
        SaveTextureAsPNG(RawTexture(forestProbabilityMap.Map), "forest_map.png");

        for (int x = 0; x < forestProbabilityMap.width; x++) {
            for (int y = 0; y < forestProbabilityMap.height; y++) {
                float val = forestProbabilityMap.At(x, y);
                float rng = Random.Range(0.55f, 0.90f);
                if (rng <= val) {
                    map[x, y] = TileEnum.forest;
                }
            }
        }
        // now, pattern match and create deepForest, where a forest is touching a forest in each cardinal direction
        // TODO: this is currently replacing while going - modify to use another method here
        // we're getting checkerboard
        PatternReplaceSingle(TerrainPatternShape.CenterPlus, TileEnum.forest, TileEnum.deepForest,
                             TileEnum.forest, TileEnum.mountain, TileEnum.mountain2x2, TileEnum.peak, TileEnum.peak2x2);    

        // add PoI and roads to them
        // create villages
        // For now, simply spawn where there aren't moutains/water
        // in the future, have villages "prefer" certain areas, maybe using GetRidges(), or find places close to water, etc
        // this may be relegated to different types of villages
        int[,] spawnableVillageMask = (elevation as NoiseMap).GetRidges().BinaryThreshold(0.80f).Subtract( map.LocationsOf<TileEnum>(TileEnum.water, TileEnum.deepWater) );
        List<Vector2Int> villagePositions = spawnableVillageMask.Where(it => it == 1).ToList().RandomSelections<Vector2Int>(numVillages);
        //
        foreach(Vector2Int pos in villagePositions) {
            map[pos.x, pos.y] = TileEnum.village;
        }

        Vector3Int prevPos = Vector3Int.zero;
		int i = 0;
		foreach (Vector2Int _pos in villagePositions.OrderBy(it => it.y)) {
            Vector3Int pos = new Vector3Int(_pos.x, _pos.y, 0);
            
			if (i > 0) {
                Path path = new ElevationPathfinder(elevation).BFS(prevPos, pos);
				
				// while we're here, update the grid for the first pass
				foreach (Vector3Int p in path.Unwind()) {
					map[p.x, p.y] = TileEnum.none;
				}
			}
			prevPos = pos;
			i++;
		}
		
        // pattern replacer is run in the base Preprocessing class
        base.Preprocessing();
    }

    private NoiseMap GeneratePerlinNoiseMap(int dimX, int dimY, int numOctaves, float scale = 1.0f, float power = 1.0f) {
        NoiseMap noiseMap = new NoiseMap(dimX, dimY, numOctaves);
        
        // When changing noise scale, it zooms from top-right corner
        // This will make it zoom from the center
        Vector2 midpoint = new Vector2(dimX/2f, dimY/2f);

        if (seed != -1) Random.InitState(seed);
        Vector2 randomOffset = new Vector2(Random.Range(-10000, 10000), Random.Range(-10000, 10000));
    
        for (int x = 0; x < dimX; x++) {
            for (int y = 0; y < dimY; y++) {

                // fill out each octave x-y
                for (int o = 0; o < numOctaves; o++) {         
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