using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilesResourcesLoader : MonoBehaviour
{
	public static Tile GetGrassTile() {
		return GetTile("grass_tile");
	}
	
	public static Tile GetDirtTile() {
		return GetTile("dirt_tile");
	}
	
	public static Tile GetWaterTile() {
		return GetTile("water_tile");
	}
	
	private static Tile GetTile(string name) {
		return (Tile)Resources.Load(name, typeof(Tile));
	}
}
