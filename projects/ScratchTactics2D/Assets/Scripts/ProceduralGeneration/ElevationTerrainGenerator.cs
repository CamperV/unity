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

public abstract class ElevationTerrainGenerator : TerrainGenerator
{
    public ElevationMap elevation;
    
    void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            GenerateMap();
            ApplyMap(GameManager.inst.overworld.baseTilemap);
        }
    }

    // these are the elevtaion thresholds for placing each tile
    protected Dictionary<float, TileEnum> tileElevation = new Dictionary<float, TileEnum>{
        [0.20f] = TileEnum.deepWater,
        [0.25f] = TileEnum.water,
        [0.30f] = TileEnum.sand,
        [0.65f] = TileEnum.plain,
        [0.75f] = TileEnum.foothill,
        [0.80f] = TileEnum.mountain,
        [1.00f] = TileEnum.peak
    };

    protected Dictionary<float, Color> colorElevation = new Dictionary<float, Color>{
        /* .00 -> */ [0.20f] = new Color(0.00f, 0.00f, 0.50f, 1.00f),    // deep blue
        /* .20 -> */ [0.25f] = new Color(0.00f, 0.00f, 1.00f, 1.00f),    // blue
        /* .25 -> */ [0.30f] = new Color(0.85f, 0.75f, 0.50f, 1.00f),    // tan
        /* .30 -> */ [0.55f] = new Color(0.00f, 1.00f, 0.00f, 1.00f),    // green
        /* .55 -> */ [0.65f] = new Color(0.00f, 0.75f, 0.00f, 1.00f),    // deep green
        /* .65 -> */ [0.75f] = new Color(0.50f, 0.50f, 0.50f, 1.00f),    // gray
        /* .75 -> */ [1.00f] = new Color(1.00f, 1.00f, 1.00f, 1.00f)     // white
    };

    protected override void Preprocessing() {
        // link the villages via roads
		CreateRoadsBetweenWaypoints( map.LocationsOf<TileEnum>(TileEnum.village)
										.Where(it => it == 1)
										.Select(it => new Vector3Int(it.x, it.y, 0))
										.ToList() );
        
        // mountain pattern replacers in TerrainGenerator
        base.Preprocessing();
    }

    // what makes this protected?
    // A: the usage of "elevation" as the pathfinding mechanism
    protected void CreateRoadsBetweenWaypoints(List<Vector3Int> waypoints) {	
        // This needs post-processing to set the appropriate tile afterwards
        Vector3Int prevPos = Vector3Int.zero;
        int i = 0;

        // create the roads in between the waypoints here
        foreach (Vector2Int _pos in waypoints.OrderBy(it => it.y)) {
            Vector3Int pos = new Vector3Int(_pos.x, _pos.y, 0);

            if (i > 0) {
                Path path = new ElevationPathfinder(elevation).BFS<Path>(prevPos, pos);
                
                // while we're here, update the grid for the first pass
                foreach (Vector3Int p in path.Unwind()) {
                    switch (map[p.x, p.y]) {
                        case TileEnum.water:
                        case TileEnum.deepWater:
                            map[p.x, p.y] = TileEnum.waterRoad;
                            break;
                        case TileEnum.forest:
                        case TileEnum.deepForest:
                            map[p.x, p.y] = TileEnum.forestRoad;
                            break;
                        case TileEnum.mountain:
                        case TileEnum.peak:
                            map[p.x, p.y] = TileEnum.mountainRoad;
                            break;
                        case TileEnum.village:
                            map[p.x, p.y] = TileEnum.villageRoad;
                            break;
                        default:
                            map[p.x, p.y] = TileEnum.road;
                            break;
                    }
                }
            }
            prevPos = pos;
            i++;
        }
    }

    protected TileEnum ElevationToTile(float elevation) {
        foreach (KeyValuePair<float, TileEnum> elPair in tileElevation.OrderBy(p => p.Key)) {
            if (elevation <= elPair.Key)
                return elPair.Value;
        }
        // default case
        return TileEnum.x;
    }

    protected Color ElevationToColor(float elevation) {
        foreach (KeyValuePair<float, Color> elPair in colorElevation.OrderBy(p => p.Key)) {
            if (elevation <= elPair.Key)
                return elPair.Value;
        }
        // default case is > 1.0
        return colorElevation[1.0f];
    }

    protected float[,] GenerateCircularGradient(float threshold = 1.0f, float slope = 1.0f) {
        float[,] gradient = new float[mapDimension.x, mapDimension.y];
        Vector2 midpoint = new Vector2(mapDimension.x/2f, mapDimension.y/2f);

        float maxDistance = Vector2.Distance(new Vector2(0, 0), midpoint);
        for (int x = 0; x < mapDimension.x; x++) {
            for (int y = 0; y < mapDimension.y; y++) {
                float distance = Vector2.Distance(new Vector2(x, y), midpoint);
                gradient[x, y] = Mathf.Clamp((1f - (Mathf.InverseLerp(0f, maxDistance, distance) * slope)), 0.0f, threshold);
            }
        }
        return gradient;
    }

    protected float[,] GenerateVerticalGradient(float threshold = 1.0f, float slope = 1.0f) {
        float[,] gradient = new float[mapDimension.x, mapDimension.y];

        float maxDistance = slope * mapDimension.y;
        for (int x = 0; x < mapDimension.x; x++) {
            for (int y = 0; y < mapDimension.y; y++) {
                float distance = slope * (mapDimension.y - y);
                gradient[x, y] = Mathf.Clamp(1f - Mathf.InverseLerp(0f, maxDistance, distance), 0.0f, threshold);
            }
        }
        return gradient;
    }

    protected float[,] GenerateLogGradient(float scale = 1.0f) {
        float[,] gradient = new float[mapDimension.x, mapDimension.y];
        float logScale = 0.001f;

        float minDistance = Mathf.Log(0.01f);
        float maxDistance = Mathf.Log(mapDimension.y * logScale);
        for (int x = 0; x < mapDimension.x; x++) {
            for (int y = 0; y < mapDimension.y; y++) {
                float distance = Mathf.Log(y * logScale);
                gradient[x, y] = scale * Mathf.InverseLerp(minDistance, maxDistance, distance);
            }
        }
        return gradient;
    }

    protected float[,] GenerateExpGradient(float power, float scale = 1.0f, bool reversed = false) {
        float[,] gradient = new float[mapDimension.x, mapDimension.y];

        float minDistance = 0f;
        float maxDistance = Mathf.Pow(mapDimension.y, power);

        for (int x = 0; x < mapDimension.x; x++) {
            for (int y = 0; y < mapDimension.y; y++) {
                float _base = (reversed) ? y :  mapDimension.y - y;
                float distance = Mathf.Pow(_base, power);
                gradient[x, y] = scale * Mathf.InverseLerp(minDistance, maxDistance, distance);
            }
        }
        return gradient;
    }

    protected float[,] GenerateRandomDimples(int verticalThreshold = 0) {
        float[,] dimpleMap = new float[mapDimension.x, mapDimension.y];
        int numDimples = Random.Range(0, (int)(mapDimension.y/20f));

        for (int d = 0; d < numDimples; d++) {
            Vector2Int randomPosition = new Vector2Int(Random.Range(0, mapDimension.x), Random.Range(0, mapDimension.y));
            int randomRadius = Random.Range(5, (int)(mapDimension.x/5.0f));

            // see if we can simply discard this one
            if ((randomPosition.y - randomRadius) < verticalThreshold) continue;

            Vector2Int minBox = randomPosition - (randomRadius * Vector2Int.one);
            Vector2Int maxBox = randomPosition + (randomRadius * Vector2Int.one);

            // only search in a box around the dimple's origin
            for (int x = Mathf.Max(0, minBox.x); x < Mathf.Min(mapDimension.x, maxBox.x); x++) {
                for (int y = Mathf.Max(0, minBox.y); y < Mathf.Min(mapDimension.y, maxBox.y); y++) {
                    float verticalScale = 0.5f + Mathf.InverseLerp(verticalThreshold, mapDimension.y, y); // 1 - 1.5 depending on vertical height

                    float distance = (new Vector2(x, y) - randomPosition).magnitude;
                    dimpleMap[x, y] += Mathf.Pow((1f - Mathf.InverseLerp(0, randomRadius, distance)), verticalScale);
                }
            }
        }
        return dimpleMap;
    }

    protected Texture2D LoadTemplate(string name) {
        return Resources.Load<Texture2D>(name);
    }

    protected float[,] TextureAsFloat(Texture2D texture) {
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

    protected Texture2D RawTexture(float[,] floatMap) {
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

    protected Texture2D ColorizedTexture(float[,] floatMap) {
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

    protected void SaveTextureAsPNG(Texture2D texture, string name) {
        byte[] bytes = texture.EncodeToPNG();
        
        var path = Application.dataPath + "/../SavedImages/";
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }
        System.IO.File.WriteAllBytes(path + name, bytes);
        //
        Debug.Log($"Saved {name} to {path}");
    }
}
