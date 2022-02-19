using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

//-------------------------------------------------------------------//
public class VisualTile : Tile
{
#if UNITY_EDITOR
    [MenuItem("Assets/Create/CustomAssets/VisualTile", false, 1)]
	private static void Create(){
		string path = EditorUtility.SaveFilePanelInProject("Save VisualTile", "VisualTile", "asset", "Save VisualTile", "Custom Assets/Tiles");
		// check that path exists in project
		if (path == "") {
			Debug.Log($"Path {path} is not available in Project to create a new VisualTile Instance");
			return;
		}

		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<VisualTile>(), path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
#endif
}