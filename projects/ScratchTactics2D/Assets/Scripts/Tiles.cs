using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public abstract class WorldTile : Tile
{
	// returns an integer that signifies the cost of entering this tile
	public abstract int GetCost();
	public int cost { get { return GetCost(); } }
	
	// returns a probability from 0-99 that this tile is generated
	public abstract int GetProbability();
	public int probability { get { return GetProbability(); } }
	public List<Sprite> sprites;
}

public abstract class OverlayTile : Tile
{
	public abstract Enum.TileLevel GetLevel();
	public Enum.TileLevel level { get { return GetLevel(); } }
}

//
// --------------
public class GrassWorldTile : WorldTile
{
	public override int GetCost() {
		return 1;
	}
	
	public override int GetProbability() {
		return 4;	// x/6
	}
	
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("grass_tile")
		};
		sprite = sprites[0];
	}
}

public class DirtWorldTile : WorldTile
{
	public override int GetCost() {
		return 1;
	}
	
	public override int GetProbability() {
		return 3;	// x/6
	}
	
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("dirt_tile")
		};
		sprite = sprites[0];
	}
}

public class WaterWorldTile : WorldTile
{
	public override int GetCost() {
		return 2;
	}
	
	public override int GetProbability() {
		return 0;	// x/6
	}
	
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("water_tile")
		};
		sprite = sprites[0];
	}
}

public class MountainWorldTile : WorldTile
{
	public override int GetCost() {
		return 5; // read: impassable
	}
	
	public override int GetProbability() {
		return 2;	// x/6
	}
	
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("mountain_tile")
		};
		sprite = sprites[0];
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