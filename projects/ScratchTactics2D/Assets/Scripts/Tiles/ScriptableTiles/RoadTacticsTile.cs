using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RoadTacticsTile : TacticsTile
{
#if UNITY_EDITOR
    [MenuItem("Assets/Create/CustomAssets/RoadTacticsTile", false, 1)]
	private static void Create(){
		string path = EditorUtility.SaveFilePanelInProject("Save RoadTacticsTile", "RoadTacticsTile", "asset", "Save RoadTacticsTile", "Custom Assets/Tiles");
		// check that path exists in project
		if (path == "") {
			Debug.Log($"Path {path} is not available in Project to create a new RoadTacticsTile Instance");
			return;
		}

		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<RoadTacticsTile>(), path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
#endif
}