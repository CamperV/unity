using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

//-------------------------------------------------------------------//
public class HealTile : TerrainTile
{
#if UNITY_EDITOR
    [MenuItem("Assets/Create/CustomAssets/HealTile", false, 1)]
	private static void Create(){
		string path = EditorUtility.SaveFilePanelInProject("Save HealTile", "HealTile", "asset", "Save HealTile", "Custom Assets/Tiles");
		// check that path exists in project
		if (path == "") {
			Debug.Log($"Path {path} is not available in Project to create a new HealTile Instance");
			return;
		}

		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<HealTile>(), path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
#endif
}