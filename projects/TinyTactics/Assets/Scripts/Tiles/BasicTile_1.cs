using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

//-------------------------------------------------------------------//
public class BasicTile_1 : TerrainTile
{
#if UNITY_EDITOR
    [MenuItem("Assets/Create/CustomAssets/BasicTile_1", false, 1)]
	private static void Create(){
		string path = EditorUtility.SaveFilePanelInProject("Save BasicTile_1", "BasicTile_1", "asset", "Save BasicTile_1", "Custom Assets/Tiles");
		// check that path exists in project
		if (path == "") {
			Debug.Log($"Path {path} is not available in Project to create a new BasicTile_1 Instance");
			return;
		}

		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<BasicTile_1>(), path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
#endif
}