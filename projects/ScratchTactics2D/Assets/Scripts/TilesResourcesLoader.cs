using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilesResourcesLoader : MonoBehaviour
{
	public static Tile getGrassTile() {
		return getTile("grass_tile");
	}
	
	public static Tile getDirtTile() {
		return getTile("dirt_tile");
	}
	
	public static Tile getWaterTile() {
		return getTile("water_tile");
	}
	
	private static Tile getTile(string name) {
		return (Tile)Resources.Load(name, typeof(Tile));
	}
}
