using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class GrassWorldTile : WorldTile
{
	public override int cost { get { return (int)(Constants.standardTickCost * 1.0f); } }
	public override int probability { get { return 73; } }
	
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("Parchment/parchment_base")
		};
		sprite = sprites[0];
	}
}

public class DirtWorldTile : WorldTile
{
	public override int cost { get { return (int)(Constants.standardTickCost * 1.5f); } }

	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("dirt_tile")
		};
		sprite = sprites[0];
	}
}

public class VillageWorldTile : WorldTile
{
	public override int cost { get { return (int)(Constants.standardTickCost * 1.0f); } }
	public override int depth { get { return 1; } }

	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("Parchment/parchment_village_base")
		};
		sprite = sprites[0];
	}
}

public class RuinsWorldTile : WorldTile
{
	public override int cost { get { return (int)(Constants.standardTickCost * 1.0f); } }
	public override int probability { get { return 1; } }

	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("Parchment/parchment_ruins")
		};
		sprite = sprites[0];
	}
}

public class ForestWorldTile : WorldTile
{
	public override int probability { get { return 8; } }
	public override int depth { get { return 1; } }
	public override int cost { get { return (int)(Constants.standardTickCost * 2.0f); } }

	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("Parchment/parchment_copse")
		};
		sprite = sprites[0];
	}
}

public class WaterWorldTile : WorldTile
{
	public override int probability { get { return 5; } }
	public override int cost { get { return (int)(Constants.standardTickCost * 5.0f); } }
	
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("Parchment/parchment_water")
		};
		sprite = sprites[0];
	}
}

public class DeepWaterWorldTile : WorldTile
{
	public override int cost { get { return (int)(Constants.standardTickCost * 10.0f); } }
	
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
	public override int depth { get { return 1; } }
	public override int cost { get { return (int)(Constants.standardTickCost * 5.0f); } }
	
	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite("Parchment/parchment_mountain_cluster")
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
	public override int cost { get { return Constants.standardTickCost; } }

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
	public override int depth { get { return 1; } }

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
	public override int depth { get { return 1; } }

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
	public override int cost { get { return Constants.standardTickCost; } }

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
			ResourceLoader.GetMultiSprite("Parchment/parchment_village_road", "parchment_village_road_10")
		};
		sprite = sprites[0];
	}
	
	public static VillageRoadWorldTile GetTileWithSprite(int spriteIndex) {
		VillageRoadWorldTile wt = ScriptableObject.CreateInstance<VillageRoadWorldTile>() as VillageRoadWorldTile;
		wt.SetSprite(spriteIndex);
		return wt;
	}
}