﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//----------------------------------------------------------------//
public abstract class WorldTile : Tile
{
	// returns a probability from 0-99 that this tile is generated
	public virtual int probability { get => 0; }
	
	// utility methods for getting specific worldtile sprites
	public List<Sprite> sprites;
	public void SetSprite(int i) {
		sprite = sprites[i];
	}
}
//----------------------------------------------------------------//

public class GrassWorldTile : WorldTile
{
	public override int probability { get { return 73; } }
	
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("Parchment/parchment_base")
		};
		sprite = sprites[0];
	}
}

public class SandWorldTile : WorldTile
{
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("Parchment/parchment_sand")
		};
		sprite = sprites[0];
	}
}

// POI
public class VillageWorldTile : WorldTile
{
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("Parchment/parchment_village_base")
		};
		sprite = sprites[0];
	}
}

// POI
public class RuinsWorldTile : WorldTile
{
	public override int probability { get { return 1; } }

	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("Parchment/parchment_ruins")
		};
		sprite = sprites[0];
	}
}

// POI
public class FortressWorldTile : WorldTile
{
	public override int probability { get { return 1; } }

	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("Parchment/parchment_fortress")
		};
		sprite = sprites[0];
	}
}

// POI
public class CampWorldTile : WorldTile
{
	public override int probability { get { return 1; } }

	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("Parchment/parchment_camp")
		};
		sprite = sprites[0];
	}
}

// POI
public class BanditCampWorldTile : WorldTile
{
	public override int probability { get { return 1; } }

	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("Parchment/parchment_bandit_camp")
		};
		sprite = sprites[0];
	}
}

// POI
public class BossBanditCampWorldTile : WorldTile
{
	public override int probability { get { return 1; } }

	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("Parchment/parchment_boss_bandit_camp")
		};
		sprite = sprites[0];
	}
}

public class ForestWorldTile : WorldTile
{
	public override int probability { get { return 8; } }

	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("Parchment/parchment_sparse_forest")
		};
		sprite = sprites[0];
	}
}

public class DeepForestWorldTile : WorldTile
{
	public override int probability { get { return 8; } }

	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("Parchment/parchment_copse")
		};
		sprite = sprites[0];
	}
}

public class FoothillsWorldTile : WorldTile
{
	public override int probability { get { return 8; } }

	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("Parchment/parchment_foothills")
		};
		sprite = sprites[0];
	}
}

public class WaterWorldTile : WorldTile
{
	public override int probability { get { return 5; } }
	
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("Parchment/parchment_water")
		};
		sprite = sprites[0];
	}
}

public class DeepWaterWorldTile : WorldTile
{	
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("Parchment/parchment_deep_water")
		};
		sprite = sprites[0];
	}
}

public class MountainWorldTile : WorldTile
{	
	public override int probability { get { return 13; } }

	public virtual void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("Parchment/parchment_mountain")
		};
		sprite = sprites[0];
	}
}

public class PeakWorldTile : MountainWorldTile
{		
	public override void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("Parchment/parchment_mountain_peaks")
		};
		sprite = sprites[0];
	}
}

public class Mountain2x2WorldTile : MountainWorldTile
{		
	public override void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("Parchment/parchment_mountain_2x2")
		};
		sprite = sprites[0];
	}
}

public class Peak2x2WorldTile : MountainWorldTile
{		
	public override void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("Parchment/parchment_mountain_peaks_2x2")
		};
		sprite = sprites[0];
	}
}

public class XWorldTile : WorldTile
{		
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("Parchment/parchment_x")
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
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetMultiSprite("Parchment/parchment_base_road", "parchment_base_road_0"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_base_road", "parchment_base_road_1"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_base_road", "parchment_base_road_2"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_base_road", "parchment_base_road_3"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_base_road", "parchment_base_road_4"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_base_road", "parchment_base_road_5"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_base_road", "parchment_base_road_6"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_base_road", "parchment_base_road_7"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_base_road", "parchment_base_road_8"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_base_road", "parchment_base_road_9"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_base_road", "parchment_base_road_10")
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
			ResourceLoader.GetMultiSprite("Parchment/parchment_mountain_road", "parchment_mountain_road_0"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_mountain_road", "parchment_mountain_road_1"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_mountain_road", "parchment_mountain_road_2"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_mountain_road", "parchment_mountain_road_3"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_mountain_road", "parchment_mountain_road_4"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_mountain_road", "parchment_mountain_road_5"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_mountain_road", "parchment_mountain_road_6"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_mountain_road", "parchment_mountain_road_7"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_mountain_road", "parchment_mountain_road_8"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_mountain_road", "parchment_mountain_road_9"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_mountain_road", "parchment_mountain_road_10")
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
			ResourceLoader.GetMultiSprite("Parchment/parchment_bridge", "parchment_bridge_0"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_bridge", "parchment_bridge_1"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_bridge", "parchment_bridge_2"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_bridge", "parchment_bridge_3"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_bridge", "parchment_bridge_4"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_bridge", "parchment_bridge_5"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_bridge", "parchment_bridge_6"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_bridge", "parchment_bridge_7"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_bridge", "parchment_bridge_8"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_bridge", "parchment_bridge_9"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_bridge", "parchment_bridge_10")
		};
		sprite = sprites[0];
	}

	public static WaterRoadWorldTile GetTileWithSprite(int spriteIndex) {
		WaterRoadWorldTile wt = ScriptableObject.CreateInstance<WaterRoadWorldTile>() as WaterRoadWorldTile;
		wt.SetSprite(spriteIndex);
		return wt;
	}
}

public class ForestRoadWorldTile : WorldTile
{
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetMultiSprite("Parchment/parchment_copse_road", "parchment_copse_road_0"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_copse_road", "parchment_copse_road_1"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_copse_road", "parchment_copse_road_2"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_copse_road", "parchment_copse_road_3"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_copse_road", "parchment_copse_road_4"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_copse_road", "parchment_copse_road_5"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_copse_road", "parchment_copse_road_6"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_copse_road", "parchment_copse_road_7"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_copse_road", "parchment_copse_road_8"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_copse_road", "parchment_copse_road_9"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_copse_road", "parchment_copse_road_10")	
		};
		sprite = sprites[0];
	}

	public static ForestRoadWorldTile GetTileWithSprite(int spriteIndex) {
		ForestRoadWorldTile wt = ScriptableObject.CreateInstance<ForestRoadWorldTile>() as ForestRoadWorldTile;
		wt.SetSprite(spriteIndex);
		return wt;
	}
}

public class VillageRoadWorldTile : WorldTile
{
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetMultiSprite("Parchment/parchment_village_road", "parchment_village_road_0"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_village_road", "parchment_village_road_1"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_village_road", "parchment_village_road_2"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_village_road", "parchment_village_road_3"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_village_road", "parchment_village_road_4"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_village_road", "parchment_village_road_5"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_village_road", "parchment_village_road_6"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_village_road", "parchment_village_road_7"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_village_road", "parchment_village_road_8"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_village_road", "parchment_village_road_9"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_village_road", "parchment_village_road_10"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_village_road", "parchment_village_road_11"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_village_road", "parchment_village_road_12"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_village_road", "parchment_village_road_13"),
			ResourceLoader.GetMultiSprite("Parchment/parchment_village_road", "parchment_village_road_14")
		};
		sprite = sprites[0];
	}
	
	public static VillageRoadWorldTile GetTileWithSprite(int spriteIndex) {
		VillageRoadWorldTile wt = ScriptableObject.CreateInstance<VillageRoadWorldTile>() as VillageRoadWorldTile;
		wt.SetSprite(spriteIndex);
		return wt;
	}
}