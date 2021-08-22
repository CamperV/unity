using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GrassRockTacticsTile : TacticsTile
{
	public override int cost { get => -1; }
	// public override float zHeight { get => 2; }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/CustomAssets/GrassRockTacticsTile", false, 1)]
	private static void Create(){
		string path = EditorUtility.SaveFilePanelInProject("Save GrassRockTacticsTile", "GrassRockTacticsTile", "asset", "Save GrassRockTacticsTile", "Custom Assets/Tiles");
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