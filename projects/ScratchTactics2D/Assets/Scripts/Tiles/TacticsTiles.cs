using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

//-------------------------------------------------------------------//
public abstract class TacticsTile : Tile
{
	// returns an integer that signifies the cost of entering this tile
	public virtual int cost { get => Constants.standardTickCost; }

	public abstract string spriteName { get; }
	public List<Sprite> sprites;

	public void OnEnable() {
		sprites = new List<Sprite>() {
			ResourceLoader.GetSprite($"Tiles/{spriteName}")
		};
		sprite = sprites[0];
	}
}
//-------------------------------------------------------------------//

public class GrassTacticsTile : TacticsTile
{
	public override string spriteName { get => "grass_tile_iso_0";}

#if UNITY_EDITOR
    [MenuItem("Assets/Create/CustomAssets/GrassTacticsTile", false, 1)]
	private static void Create(){
		string path = EditorUtility.SaveFilePanelInProject("Save GrassTacticsTile", "GrassTacticsTile", "Asset", "Save GrassTacticsTile", "Custom Assets/Tiles");
		// check that path exists in project
		if (path == "") {
			Debug.Log($"Path {path} is not available in Project to create a new GrassTacticsTile Instance");
			return;
		}

		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<GrassTacticsTile>(), path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
#endif
}

public class GrassRoughTacticsTile : TacticsTile
{
	public override string spriteName { get => "grass_tile_iso_1"; }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/CustomAssets/GrassRoughTacticsTile", false, 1)]
	private static void Create(){
		string path = EditorUtility.SaveFilePanelInProject("Save GrassRoughTacticsTile", "GrassRoughTacticsTile", "Asset", "Save GrassRoughTacticsTile", "Custom Assets/Tiles");
		// check that path exists in project
		if (path == "") {
			Debug.Log($"Path {path} is not available in Project to create a new GrassRoughTacticsTile Instance");
			return;
		}

		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<GrassRoughTacticsTile>(), path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
#endif
}

public class GrassRockTacticsTile : TacticsTile
{
	public override string spriteName { get => "rock_grass_tile_iso_0"; }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/CustomAssets/GrassRockTacticsTile", false, 1)]
	private static void Create(){
		string path = EditorUtility.SaveFilePanelInProject("Save GrassRockTacticsTile", "GrassRockTacticsTile", "Asset", "Save GrassRockTacticsTile", "Custom Assets/Tiles");
		// check that path exists in project
		if (path == "") {
			Debug.Log($"Path {path} is not available in Project to create a new GrassRockTacticsTile Instance");
			return;
		}

		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<GrassRockTacticsTile>(), path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
#endif
}

public class ForestTacticsTile : TacticsTile
{
	public override string spriteName { get => "forest_tile_iso_0"; }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/CustomAssets/ForestTacticsTile", false, 1)]
	private static void Create(){
		string path = EditorUtility.SaveFilePanelInProject("Save ForestTacticsTile", "ForestTacticsTile", "Asset", "Save ForestTacticsTile", "Custom Assets/Tiles");
		// check that path exists in project
		if (path == "") {
			Debug.Log($"Path {path} is not available in Project to create a new ForestTacticsTile Instance");
			return;
		}

		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<ForestTacticsTile>(), path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
#endif
}

public class ForestBrushTacticsTile : TacticsTile
{
	public override string spriteName { get => "brush_forest_tile_iso_0"; }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/CustomAssets/ForestBrushTacticsTile", false, 1)]
	private static void Create(){
		string path = EditorUtility.SaveFilePanelInProject("Save ForestBrushTacticsTile", "ForestBrushTacticsTile", "Asset", "Save ForestBrushTacticsTile", "Custom Assets/Tiles");
		// check that path exists in project
		if (path == "") {
			Debug.Log($"Path {path} is not available in Project to create a new ForestBrushTacticsTile Instance");
			return;
		}

		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<ForestBrushTacticsTile>(), path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
#endif
}

public class ForestTreeTacticsTile : TacticsTile
{
	public override string spriteName { get => "tree_forest_tile_iso_0"; }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/CustomAssets/ForestTreeTacticsTile", false, 1)]
	private static void Create(){
		string path = EditorUtility.SaveFilePanelInProject("Save ForestTreeTacticsTile", "ForestTreeTacticsTile", "Asset", "Save ForestTreeTacticsTile", "Custom Assets/Tiles");
		// check that path exists in project
		if (path == "") {
			Debug.Log($"Path {path} is not available in Project to create a new ForestTreeTacticsTile Instance");
			return;
		}

		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<ForestTreeTacticsTile>(), path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
#endif
}

public class WaterTacticsTile : TacticsTile
{
	public override string spriteName { get => "water_tile_iso_0"; }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/CustomAssets/WaterTacticsTile", false, 1)]
	private static void Create(){
		string path = EditorUtility.SaveFilePanelInProject("Save WaterTacticsTile", "WaterTacticsTile", "Asset", "Save WaterTacticsTile", "Custom Assets/Tiles");
		// check that path exists in project
		if (path == "") {
			Debug.Log($"Path {path} is not available in Project to create a new WaterTacticsTile Instance");
			return;
		}

		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<WaterTacticsTile>(), path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
#endif
}

public class MountainTacticsTile : TacticsTile
{
	public override string spriteName { get => "mountain_tile_iso_0"; }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/CustomAssets/MountainTacticsTile", false, 1)]
	private static void Create(){
		string path = EditorUtility.SaveFilePanelInProject("Save MountainTacticsTile", "MountainTacticsTile", "Asset", "Save MountainTacticsTile", "Custom Assets/Tiles");
		// check that path exists in project
		if (path == "") {
			Debug.Log($"Path {path} is not available in Project to create a new MountainTacticsTile Instance");
			return;
		}

		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<MountainTacticsTile>(), path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
#endif
}

// OVERLAY ZONE
public class SpawnMarkerTacticsTile : TacticsTile
{
	public override string spriteName { get => "spawn_marker_iso"; }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/CustomAssets/SpawnMarkerTacticsTile", false, 1)]
	private static void Create(){
		string path = EditorUtility.SaveFilePanelInProject("Save SpawnMarkerTacticsTile", "SpawnMarkerTacticsTile", "Asset", "Save SpawnMarkerTacticsTile", "Custom Assets/Tiles");
		// check that path exists in project
		if (path == "") {
			Debug.Log($"Path {path} is not available in Project to create a new SpawnMarkerTacticsTile Instance");
			return;
		}

		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<SpawnMarkerTacticsTile>(), path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
#endif
}

public class DockingMarkerTacticsTile : TacticsTile
{
	public override string spriteName { get => "docking_marker_iso"; }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/CustomAssets/DockingMarkerTacticsTile", false, 1)]
	private static void Create(){
		string path = EditorUtility.SaveFilePanelInProject("Save DockingMarkerTacticsTile", "DockingMarkerTacticsTile", "Asset", "Save DockingMarkerTacticsTile", "Custom Assets/Tiles");
		// check that path exists in project
		if (path == "") {
			Debug.Log($"Path {path} is not available in Project to create a new DockingMarkerTacticsTile Instance");
			return;
		}

		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<DockingMarkerTacticsTile>(), path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
#endif
}