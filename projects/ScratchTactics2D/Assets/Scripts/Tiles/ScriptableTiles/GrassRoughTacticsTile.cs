using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GrassRoughTacticsTile : TacticsTile
{
	//public override string spriteName { get => "grass_tile_iso_1"; }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/CustomAssets/GrassRoughTacticsTile", false, 1)]
	private static void Create(){
		string path = EditorUtility.SaveFilePanelInProject("Save GrassRoughTacticsTile", "GrassRoughTacticsTile", "asset", "Save GrassRoughTacticsTile", "Custom Assets/Tiles");
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