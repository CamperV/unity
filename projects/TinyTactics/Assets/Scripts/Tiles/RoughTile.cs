using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

//-------------------------------------------------------------------//
public class RoughTile : TerrainTile
{
#if UNITY_EDITOR
    [MenuItem("Assets/Create/CustomAssets/RoughTile", false, 1)]
	private static void Create(){
		string path = EditorUtility.SaveFilePanelInProject("Save RoughTile", "RoughTile", "asset", "Save RoughTile", "Custom Assets/Tiles");
		// check that path exists in project
		if (path == "") {
			Debug.Log($"Path {path} is not available in Project to create a new RoughTile Instance");
			return;
		}

		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<RoughTile>(), path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
#endif
}