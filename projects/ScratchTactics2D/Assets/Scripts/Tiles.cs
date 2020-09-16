using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public abstract class WorldTile : Tile
{
	public abstract int GetCost();
	public int cost { get { return GetCost(); } }
}

public abstract class OverlayTile : Tile
{
	public enum Level {world, overlay, super};
	public abstract Level GetLevel();
	public Level level { get { return GetLevel(); } }
}

//
// --------------
public class DirtWorldTile : WorldTile
{
	public override int GetCost() {
		return 1;
	}
	
	public void OnEnable() {
		sprite = ResourceLoader.GetSprite("dirt_tile");
	}
}

public class GrassWorldTile : WorldTile
{
	public override int GetCost() {
		return 1;
	}
	
	public void OnEnable() {
		sprite = ResourceLoader.GetSprite("grass_tile");
	}
}

public class WaterWorldTile : WorldTile
{
	public override int GetCost() {
		return 5;
	}
	
	public void OnEnable() {
		sprite = ResourceLoader.GetSprite("water_tile");
	}
}

public class MountainWorldTile : WorldTile
{
	public override int GetCost() {
		return 100;
	}
	
	public void OnEnable() {
		sprite = ResourceLoader.GetSprite("mountain_tile");
	}
}

//
// --------------
public class SelectOverlayTile : OverlayTile
{
	public override Level GetLevel() {
		return Level.super;
	}
	
	public void OnEnable() {
		sprite = ResourceLoader.GetSprite("select_overlay_tile");
	}
}

public class PathOverlayTile : OverlayTile
{
	public override Level GetLevel() {
		return Level.overlay;
	}
	
	public void OnEnable() {
		sprite = ResourceLoader.GetSprite("path_dot");
	}
}