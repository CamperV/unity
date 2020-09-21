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
	public abstract Enum.TileLevel GetLevel();
	public Enum.TileLevel level { get { return GetLevel(); } }
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
	public override Enum.TileLevel GetLevel() {
		return Enum.TileLevel.super;
	}
	
	public void OnEnable() {
		sprite = ResourceLoader.GetSprite("select_overlay_tile");
	}
}

public class PathOverlayTile : OverlayTile
{
	public override Enum.TileLevel GetLevel() {
		return Enum.TileLevel.overlay;
	}
	
	public void OnEnable() {
		sprite = ResourceLoader.GetSprite("path_dot");
	}
}

public class EndpointOverlayTile : OverlayTile
{
	public override Enum.TileLevel GetLevel() {
		return Enum.TileLevel.overlay;
	}
	
	public void OnEnable() {
		sprite = ResourceLoader.GetSprite("path_end");
	}
}