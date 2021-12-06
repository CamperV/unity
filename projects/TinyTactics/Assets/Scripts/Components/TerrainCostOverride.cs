using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCostOverride : MonoBehaviour
{
    public TerrainTile[] terrainTiles;
    public int[] _overrides;

    public Dictionary<TerrainTile, int> overrides;

    void Awake() {
        Debug.Assert(terrainTiles.Length == _overrides.Length);

        overrides = new Dictionary<TerrainTile, int>();

        for (int o = 0; o < terrainTiles.Length; o++) {
            TerrainTile tt = terrainTiles[o];
            overrides[tt] = _overrides[o];
        }
    }
}
