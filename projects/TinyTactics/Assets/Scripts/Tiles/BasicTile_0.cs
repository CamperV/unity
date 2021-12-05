using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

//-------------------------------------------------------------------//
public class BasicTile_0 : TerrainTile
{
#if UNITY_EDITOR
    [MenuItem("Assets/Create/CustomAssets/BasicTile_0", false, 1)]
	private static void Create(){
		string path = EditorUtility.SaveFilePanelInProject("Save BasicTile_0", "BasicTile_0", "asset", "Save BasicTile_0", "Custom Assets/Tiles");
		// check that path exists in project
		if (path == "") {
			Debug.Log($"Path {path} is not available in Project to create a new BasicTile_0 Instance");
			return;
		}

		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<BasicTile_0>(), path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
#endif
}