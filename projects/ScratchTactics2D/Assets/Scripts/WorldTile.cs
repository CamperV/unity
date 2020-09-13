using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public abstract class WorldTile : Tile
{
	public abstract int GetCost();
	public int cost { get { return GetCost(); } }
}

public class DirtWorldTile : WorldTile
{
	public override int GetCost() {
		return 1;
	}
	
	public void OnEnable() {
		sprite = SpriteResourcesLoader.GetSprite("dirt_tile");
	}
}

public class GrassWorldTile : WorldTile
{
	public override int GetCost() {
		return 0;
	}
	
	public void OnEnable() {
		sprite = SpriteResourcesLoader.GetSprite("grass_tile");
	}
}

public class WaterWorldTile : WorldTile
{
	public override int GetCost() {
		return 5;
	}
	
	public void OnEnable() {
		sprite = SpriteResourcesLoader.GetSprite("water_tile");
	}
}

public class MountainWorldTile : WorldTile
{
	public override int GetCost() {
		return 100;
	}
	
	public void OnEnable() {
		sprite = SpriteResourcesLoader.GetSprite("mountain_tile");
	}
}

