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

public class PerlinTerrainGenerator : TerrainGenerator
{
    [Header("Perlin Noise Settings")]
	public int seed;
	public float scale;
	public int octaves;
    [Range(0, 1)]
	public float persistance;
	public float lacunarity;
	public Vector2 offset;

    void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            GenerateMap();
            ApplyMap(GameManager.inst.worldGrid.baseTilemap);
        }
    }

    // these are the elevtaion thresholds for placing each tile
    private Dictionary<float, TileEnum> tileElevation = new Dictionary<float, TileEnum>{
        [0.10f] = TileEnum.deepWater,
        [0.25f] = TileEnum.water,
        [0.35f] = TileEnum.dirt,
        [0.60f] = TileEnum.grass,
        [0.75f] = TileEnum.forest,
        [1.00f] = TileEnum.mountain
    };

    private Dictionary<float, Color> colorElevation = new Dictionary<float, Color>{
        [0.10f] = new Color(0.00f, 0.00f, 0.50f, 1.00f),    // deep blue
        [0.25f] = new Color(0.00f, 0.00f, 1.00f, 1.00f),    // blue
        [0.35f] = new Color(0.85f, 0.75f, 0.50f, 1.00f),    // tan
        [0.60f] = new Color(0.00f, 1.00f, 0.00f, 1.00f),    // green
        [0.75f] = new Color(0.50f, 0.50f, 0.50f, 1.00f),    // gray
        [1.00f] = new Color(1.00f, 1.00f, 1.00f, 1.00f)     // white
    };

	public override void GenerateMap() {
        map = new TileEnum[mapDimensionX, mapDimensionY];

        
        //Texture2D template = LoadTemplate("small_circular");
        //float[,] finalFloat = TextureAsFloat(template).AddElementWise( GenerateGradient() );
        //float[,] noise = GenerateNoiseMap(additive: TextureAsFloat(template));
        float[,] noise = GenerateNoiseMap();
        
        // save the noise as a Texture2D
        Texture2D rawTexture = RawTexture(noise);
        Texture2D terrainTexture = ColorizedTexture(noise);
        Texture2D gradTexture = RawTexture( GenerateGradient(0.5f, 1.5f) );
        SaveTextureAsPNG(rawTexture, "noise_map.png");
        SaveTextureAsPNG(terrainTexture, "terrain_map.png");
        //SaveTextureAsPNG(template, "template.png");
        //SaveTextureAsPNG( RawTexture(finalFloat), "final_noise.png");
        SaveTextureAsPNG(gradTexture, "gradient.png");
    	
		for (int i = 0; i < map.GetLength(0); i++) {
			for (int j = 0; j < map.GetLength(1); j++) {
				map[i, j] = ElevationToTile(noise[i, j]);
			}
		}
    }

    private float[,] GenerateNoiseMap(float[,] additive = null) {
        float[,] noise = new float[mapDimensionX, mapDimensionY];
        
        if (seed != -1) Random.InitState(seed);
        List<Vector2> octaveOffsets = new List<Vector2>();
        for (int i = 0; i < octaves; i++) {
            octaveOffsets.Add( new Vector2(Random.Range(-100000, 100000), Random.Range(-100000, 100000)) + offset );
        }
        
        // When changing noise scale, it zooms from top-right corner
        // This will make it zoom from the center
        Vector2 midpoint = new Vector2(mapDimensionX/2f, mapDimensionY/2f);

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;
    
        for (int x = 0; x < mapDimensionX; x++) {
            for (int y = 0; y < mapDimensionY; y++) {
                float amplitude = 1.0f;
                float frequency = 1.0f;
                float noiseHeight = 0.0f;

                foreach (var octaveOffset in octaveOffsets) {                 
                    float sampleX = (float)(x - midpoint.x)/scale;
                    float sampleY = (float)(y - midpoint.y)/scale;
                    Vector2 sample = (new Vector2(sampleX, sampleY) * frequency) + octaveOffset;
                    float perlinValue = Mathf.PerlinNoise(sample.x, sample.y);
    
                    noiseHeight += perlinValue*amplitude;
                    //noiseHeight += Mathf.Pow(perlinValue, 0.75f);
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                // assign temp non-lerped value here
                noise[x, y] = noiseHeight;
                if (additive != null) {
                    noise[x, y] += additive[x, y];
                }

                // keep track of the bounds as we create them
                maxNoiseHeight = Mathf.Max(noise[x, y], maxNoiseHeight);
                minNoiseHeight = Mathf.Min(noise[x, y], minNoiseHeight);
            }
        }

        // go for a second pass and lerp the noise values between the max/min
        for (int x = 0; x < mapDimensionX; x++) {
            for (int y = 0; y < mapDimensionY; y++) {
                noise[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noise[x, y]);

                // if (additive != null) {
                //     noise[x, y] += additive[x, y] - 0.50f;
                // }
            }
        }

        return noise;
    }

    private TileEnum ElevationToTile(float elevation) {
        foreach (KeyValuePair<float, TileEnum> elPair in tileElevation.OrderBy(p => p.Key)) {
            if (elevation <= elPair.Key)
                return elPair.Value;
        }
        // default case
        return TileEnum.x;
    }

    private Color ElevationToColor(float elevation) {
        foreach (KeyValuePair<float, Color> elPair in colorElevation.OrderBy(p => p.Key)) {
            if (elevation <= elPair.Key)
                return elPair.Value;
        }
        // default case is > 1.0
        return colorElevation[1.0f];
    }

    public override void Postprocessing() {}

    public Texture2D RawTexture(float[,] floatMap) {
        Texture2D texture = new Texture2D(floatMap.GetLength(0), floatMap.GetLength(1));
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        for (int x = 0; x < floatMap.GetLength(0); x++) {
            for (int y = 0; y < floatMap.GetLength(1); y++) {
                texture.SetPixel(x, y, Color.Lerp(Color.black, Color.white, floatMap[x, y]) );
            }
        }

        texture.Apply();
        return texture;
    }

    public Texture2D ColorizedTexture(float[,] floatMap) {
        Texture2D texture = new Texture2D(floatMap.GetLength(0), floatMap.GetLength(1));
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        for (int x = 0; x < floatMap.GetLength(0); x++) {
            for (int y = 0; y < floatMap.GetLength(1); y++) {
                texture.SetPixel(x, y, ElevationToColor(floatMap[x, y]) );
            }
        }

        texture.Apply();
        return texture;
    }

    private void SaveTextureAsPNG(Texture2D texture, string name) {
        byte[] bytes = texture.EncodeToPNG();
        
        var path = Application.dataPath + "/../SavedImages/";
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }
        System.IO.File.WriteAllBytes(path + name, bytes);
        //
        Debug.Log($"Saved {name} to {path}");
    }

    private float[,] GenerateGradient(float threshold = 1.0f, float slope = 1.0f) {
        float[,] gradient = new float[mapDimensionX, mapDimensionY];
        Vector2 midpoint = new Vector2(mapDimensionX/2f, mapDimensionY/2f);

        float maxDistance = Vector2.Distance(new Vector2(0, 0), midpoint);
        for (int x = 0; x < mapDimensionX; x++) {
            for (int y = 0; y < mapDimensionY; y++) {
                float distance = Vector2.Distance(new Vector2(x, y), midpoint);
                gradient[x, y] = Mathf.Clamp((1f - (Mathf.InverseLerp(0f, maxDistance, distance) * slope)), 0.0f, threshold);
            }
        }
        return gradient;
    }

    private Texture2D LoadTemplate(string name) {
        return Resources.Load<Texture2D>(name);
    }

    private float[,] TextureAsFloat(Texture2D texture) {
        float[,] floatMap = new float[texture.width, texture.height];

        for (int x = 0; x < floatMap.GetLength(0); x++) {
            for (int y = 0; y < floatMap.GetLength(1); y++) {
                Color colorAt = texture.GetPixel(x, y);
                Debug.Assert(colorAt.r == colorAt.g && colorAt.g == colorAt.b);

                floatMap[x, y] = colorAt.maxColorComponent;
            }
        }
        return floatMap;
    }
}