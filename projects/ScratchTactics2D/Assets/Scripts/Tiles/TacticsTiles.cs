﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class GrassIsoTile : TacticsTile
{
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("grass_tile_iso")
		};
		sprite = sprites[0];
	}
}

public class ForestIsoTile : TacticsTile
{
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("forest_tile_iso")
		};
		sprite = sprites[0];
	}
}

public class WaterIsoTile : TacticsTile
{
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("water_tile_iso")
		};
		sprite = sprites[0];
	}
}

public class MountainIsoTile : TacticsTile
{
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("mountain_tile_iso")
		};
		sprite = sprites[0];
	}
}