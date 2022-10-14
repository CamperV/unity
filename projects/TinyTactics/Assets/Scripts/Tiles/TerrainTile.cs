using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu (menuName = "CustomTiles/TerrainTile")]
public class TerrainTile : Tile
{
	// returns an integer that signifies the cost of entering this tile
	public int cost;
	public TerrainEffect terrainEffect;
	public string displayName;

	public bool HasTerrainEffect => terrainEffect != null;
}