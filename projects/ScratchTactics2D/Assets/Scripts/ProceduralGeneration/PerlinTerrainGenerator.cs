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
        float[,] lakeMask = GenerateRandomDimples(verticalThreshold: (int)(mapDimension.y/4f));
        SaveTextureAsPNG(RawTexture(lakeMask), "lakeMask.png");
        
        elevation = GeneratePerlinNoiseMap(mapDimension.x, mapDimension.y, octaves, scale: perlinScale, power: perlinPower, modulate: true);
        elevation.Add(beachMask).Add(mountainMask).Subtract(lakeMask);
        elevation.NormalizeMap();
        
        // save the noise as a Texture2D
        SaveTextureAsPNG( RawTexture(elevation.Map), "noise_map.png");
        SaveTextureAsPNG( ColorizedTexture(elevation.Map), "terrain_map.png");
    	
        map = new TileEnum[mapDimension.x, mapDimension.y];
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
        NoiseMap forestProbabilityMap = GeneratePerlinNoiseMap(mapDimension.x, mapDimension.y, 1, scale: perlinScale*2.0f, power: 0.75f);
        forestProbabilityMap.Mask( elevation.Map.ClampBinaryThreshold(0.40f, 0.85f) );
        SaveTextureAsPNG(RawTexture(forestProbabilityMap.Map), "forest_map.png");

        for (int x = 0; x < forestProbabilityMap.width; x++) {
            for (int y = 0; y < forestProbabilityMap.height; y++) {
                float val = forestProbabilityMap.At(x, y);
                float rng = Random.Range(0.40f, 0.90f);
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
        SaveTextureAsPNG( RawTexture(spawnableVillageMask.ToFloat() ), "village_map.png");
        List<Vector2Int> villagePositions = spawnableVillageMask.Where(it => it == 1).ToList().RandomSelections<Vector2Int>(numVillages);
        //
        foreach(Vector2Int pos in villagePositions) {
            map[pos.x, pos.y] = TileEnum.village;
        }

        // do Ruins here
        // 1f indicates EVERY deepForest (surrounded by deepForest) you see will be replaced with a Ruins
        // 0f means no ruins will appear
        PatternReplaceRandom(0.05f, TerrainPatternShape.CenterPlus, TileEnum.deepForest, TileEnum.ruins,
                             TileEnum.deepForest);

        // do Fortresses here
        // patternreplace touching at least one Mountain and at least one 
        // these conditions are lambdas that are run on each member of a Pattern surrounding the target
        // the second number in the Pair is how many times this must become true to be valid
        // Here: a fortress will be created if there is at least one mountain/peak touching it, and at least one NON mountain/peak touching it (5% of the time)
        var fortressCondition1 = new Pair<Func<TileEnum, bool>, int>((it) => { return it == TileEnum.mountain || it == TileEnum.peak; }, 2);
        var fortressCondition2 = new Pair<Func<TileEnum, bool>, int>((it) => { return !fortressCondition1.first(it); }, 1);

        // this defines which TileEnums to target
        Func<TileEnum, bool> whichTileEnumCondition = (it) => { return fortressCondition2.first(it); };
        PatternReplaceConditional(0.05f, TerrainPatternShape.NoCenterPlus, whichTileEnumCondition, TileEnum.fortress, fortressCondition1, fortressCondition2);
		
        // road router in ElevationTerrainGenerator
        base.Preprocessing();
    }

    private NoiseMap GeneratePerlinNoiseMap(int dimX, int dimY, int numOctaves, float scale = 1.0f, float power = 1.0f, bool modulate = false) {
        NoiseMap noiseMap = new NoiseMap(dimX, dimY, numOctaves);
        
        // When changing noise scale, it zooms from top-right corner
        // This will make it zoom from the center
        Vector2 midpoint = new Vector2(dimX/2f, dimY/2f);
        Vector2 randomOffset = new Vector2(Random.Range(-10000, 10000), Random.Range(-10000, 10000));
    
        for (int x = 0; x < dimX; x++) {
            for (int y = 0; y < dimY; y++) {

                // fill out each octave x-y
                for (int o = 0; o < numOctaves; o++) {         
                    float octaveScale = Mathf.Pow(2f, o);  // 1, 2, 4, 8, etc

                    // change the scale based on your y-location
                    // this is to make "beaches" less "mountain-y"
                    float relativeScale = scale;
                    if (modulate) {
                        float yRatio = 1f - Mathf.InverseLerp(0, dimY, y);
                        relativeScale = (yRatio*2f + 1f) * scale;
                    }

                    float sampleX = (float)(x - midpoint.x)/relativeScale;
                    float sampleY = (float)(y - midpoint.y)/relativeScale;
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