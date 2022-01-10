using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

//-------------------------------------------------------------------//
public class TerrainTile : Tile
{
	// returns an integer that signifies the cost of entering this tile
	public int cost;
	public TerrainEffect terrainEffect;
	public string displayName;

	public bool HasTerrainEffect => terrainEffect != null;

#if UNITY_EDITOR
    [MenuItem("Assets/Create/CustomAssets/TerrainTile", false, 1)]
	private static void Create(){
		string path = EditorUtility.SaveFilePanelInProject("Save TerrainTile", "TerrainTile", "asset", "Save TerrainTile", "Custom Assets/Tiles");
		// check that path exists in project
		if (path == "") {
			Debug.Log($"Path {path} is not available in Project to create a new TerrainTile Instance");
			return;
		}

		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<TerrainTile>(), path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
#endif
}