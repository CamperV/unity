using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

//-------------------------------------------------------------------//
public class ImpassableTile : TerrainTile
{
#if UNITY_EDITOR
    [MenuItem("Assets/Create/CustomAssets/ImpassableTile", false, 1)]
	private static void Create(){
		string path = EditorUtility.SaveFilePanelInProject("Save ImpassableTile", "ImpassableTile", "asset", "Save ImpassableTile", "Custom Assets/Tiles");
		// check that path exists in project
		if (path == "") {
			Debug.Log($"Path {path} is not available in Project to create a new ImpassableTile Instance");
			return;
		}

		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<ImpassableTile>(), path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
#endif
}