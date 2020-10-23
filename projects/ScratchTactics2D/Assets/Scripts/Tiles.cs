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
	
	// returns an integer that signifies the cost of entering this tile
	public virtual Vector2Int GetBattleGridSize() {
		return new Vector2Int(9, 9);
	}
	public Vector2Int battleGridSize { get { return GetBattleGridSize(); } }
	public List<Sprite> sprites;
	
	// reserved for tiles that have visual depth, ie mountains
	public virtual int GetDepth() {
		return 0;
	}
	public int depth { get { return GetDepth(); } }
	
	// utility methods for getting specific worldtile sprites
	public void SetSprite(int i) {
		sprite = sprites[i];
	}
}

public abstract class TacticsTile : Tile
{
	// returns an integer that signifies the cost of entering this tile
	public abstract int GetCost();
	public int cost { get { return GetCost(); } }
	
	public List<Sprite> sprites;
}

public abstract class OverlayTile : Tile
{
	public List<Sprite> sprites;
	public void SetSprite(int i) {
		sprite = sprites[i];
	}
}

//
// --------------
public class GrassWorldTile : WorldTile
{
	public override int GetCost() {
		return 1;
	}
	
	public override int GetProbability() {
		return 5;	// x/6
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
		return 0;	// x/6
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
		return 1;	// x/6
	}
	
	public override int GetDepth() {
		return 1;
	}
	
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("mountain_range_tile")
		};
		sprite = sprites[0];
	}
}

public class CloudWorldTile : WorldTile
{
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetMultiSprite("cloudy_grassy_mountain", "cloudy_grassy_mountain_0"),
			ResourceLoader.GetMultiSprite("cloudy_grassy_mountain", "cloudy_grassy_mountain_1"),
			ResourceLoader.GetMultiSprite("cloudy_grassy_mountain", "cloudy_grassy_mountain_2")
		};
		sprite = sprites[0];
	}
	
	public override int GetCost() {
		return -1; // read: impassable
	}
	
	public override int GetProbability() {
		return 0;	// x/6
	}
	
	public static CloudWorldTile GetTileWithSprite(int spriteIndex) {
		CloudWorldTile wt = ScriptableObject.CreateInstance<CloudWorldTile>() as CloudWorldTile;
		wt.SetSprite(spriteIndex);
		return wt;
	}
}

public class RoadWorldTile : WorldTile
{
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetMultiSprite("grass_road_tile", "grass_road_tile_0"),
			ResourceLoader.GetMultiSprite("grass_road_tile", "grass_road_tile_1"),
			ResourceLoader.GetMultiSprite("grass_road_tile", "grass_road_tile_2"),
			ResourceLoader.GetMultiSprite("grass_road_tile", "grass_road_tile_3"),
			ResourceLoader.GetMultiSprite("grass_road_tile", "grass_road_tile_4"),
			ResourceLoader.GetMultiSprite("grass_road_tile", "grass_road_tile_5")
		};
		sprite = sprites[0];
	}
	
	public override int GetCost() {
		return -1; // read: impassable
	}
	
	public override int GetProbability() {
		return 0;	// x/6
	}
	
	public static RoadWorldTile GetTileWithSprite(int spriteIndex) {
		RoadWorldTile wt = ScriptableObject.CreateInstance<RoadWorldTile>() as RoadWorldTile;
		wt.SetSprite(spriteIndex);
		return wt;
	}
}

//
// --------------
public class GrassIsoTile : TacticsTile
{
	public override int GetCost() {
		return 1;
	}
	
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("grass_tile_iso")
		};
		sprite = sprites[0];
	}
}

public class MountainIsoTile : TacticsTile
{
	public override int GetCost() {
		return 1;
	}
	
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("mountain_tile_iso")
		};
		sprite = sprites[0];
	}
}

//
// --------------
public class SelectOverlayTile : OverlayTile
{
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("select_overlay_tile")
		};
		sprite = sprites[0];
	}
	
	public static SelectOverlayTile GetTileWithSprite(int spriteIndex) {
		SelectOverlayTile wt = ScriptableObject.CreateInstance<SelectOverlayTile>() as SelectOverlayTile;
		wt.SetSprite(spriteIndex);
		return wt;
	}
}

public class PathOverlayTile : OverlayTile
{
	public void OnEnable() {
		sprite = ResourceLoader.GetSprite("path_dot");
	}
}

public class EndpointOverlayTile : OverlayTile
{
	public void OnEnable() {
		sprite = ResourceLoader.GetSprite("path_end");
	}
}

public class SelectOverlayIsoTile : OverlayTile
{
	public void OnEnable() {
		sprite = ResourceLoader.GetSprite("select_overlay_iso_tile");
	}
}