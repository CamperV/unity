using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class GrassWorldTile : WorldTile
{
	public override int probability { get { return 85; } }
	
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("grass_tile")
		};
		sprite = sprites[0];
	}
}

public class DirtWorldTile : WorldTile
{
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("dirt_tile")
		};
		sprite = sprites[0];
	}
}

public class WaterWorldTile : WorldTile
{
	public override int cost { get { return 2; } }
	public override int probability { get { return 2; } }
	
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("water_tile")
		};
		sprite = sprites[0];
	}
}

public class MountainWorldTile : WorldTile
{	
	public override int cost { get { return 10; } }
	public override int probability { get { return 13; } }
	public override int depth { get { return 1; } }
	
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
	
	public static CloudWorldTile GetTileWithSprite(int spriteIndex) {
		CloudWorldTile wt = ScriptableObject.CreateInstance<CloudWorldTile>() as CloudWorldTile;
		wt.SetSprite(spriteIndex);
		return wt;
	}
}

public class RoadWorldTile : WorldTile
{
	public override int depth { get { return 0; } }

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
	
	public static RoadWorldTile GetTileWithSprite(int spriteIndex) {
		RoadWorldTile wt = ScriptableObject.CreateInstance<RoadWorldTile>() as RoadWorldTile;
		wt.SetSprite(spriteIndex);
		return wt;
	}
}

public class MountainRoadWorldTile : WorldTile
{
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetMultiSprite("mountain_road_tile", "mountain_road_tile_0"),
			ResourceLoader.GetMultiSprite("mountain_road_tile", "mountain_road_tile_1"),
			ResourceLoader.GetMultiSprite("mountain_road_tile", "mountain_road_tile_2"),
			ResourceLoader.GetMultiSprite("mountain_road_tile", "mountain_road_tile_3"),
			ResourceLoader.GetMultiSprite("mountain_road_tile", "mountain_road_tile_4"),
			ResourceLoader.GetMultiSprite("mountain_road_tile", "mountain_road_tile_5")			
		};
		sprite = sprites[0];
	}
	
	public static MountainRoadWorldTile GetTileWithSprite(int spriteIndex) {
		MountainRoadWorldTile wt = ScriptableObject.CreateInstance<MountainRoadWorldTile>() as MountainRoadWorldTile;
		wt.SetSprite(spriteIndex);
		return wt;
	}
}

public class WaterRoadWorldTile : WorldTile
{
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetMultiSprite("water_road_tile", "water_road_tile_0"),
			ResourceLoader.GetMultiSprite("water_road_tile", "water_road_tile_1"),
			ResourceLoader.GetMultiSprite("water_road_tile", "water_road_tile_2"),
			ResourceLoader.GetMultiSprite("water_road_tile", "water_road_tile_3"),
			ResourceLoader.GetMultiSprite("water_road_tile", "water_road_tile_4"),
			ResourceLoader.GetMultiSprite("water_road_tile", "water_road_tile_5")			
		};
		sprite = sprites[0];
	}

	public static WaterRoadWorldTile GetTileWithSprite(int spriteIndex) {
		WaterRoadWorldTile wt = ScriptableObject.CreateInstance<WaterRoadWorldTile>() as WaterRoadWorldTile;
		wt.SetSprite(spriteIndex);
		return wt;
	}
}