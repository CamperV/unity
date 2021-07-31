using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class GrassIsoTile : TacticsTile
{
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("Tiles/grass_tile_iso_0")
		};
		sprite = sprites[0];
	}
}

public class ForestIsoTile : TacticsTile
{
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("Tiles/forest_tile_iso_0")
		};
		sprite = sprites[0];
	}
}

public class WaterIsoTile : TacticsTile
{
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("Tiles/water_tile_iso_0")
		};
		sprite = sprites[0];
	}
}

public class MountainIsoTile : TacticsTile
{
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("Tiles/mountain_tile_iso_0")
		};
		sprite = sprites[0];
	}
}