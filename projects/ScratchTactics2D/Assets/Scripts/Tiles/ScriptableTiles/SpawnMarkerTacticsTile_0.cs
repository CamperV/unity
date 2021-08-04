using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SpawnMarkerTacticsTile_0 : TacticsTile
{
	//public override string spriteName { get => "spawn_marker_iso_0"; }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/CustomAssets/SpawnMarkerTacticsTile_0", false, 1)]
	private static void Create(){
		string path = EditorUtility.SaveFilePanelInProject("Save SpawnMarkerTacticsTile_0", "SpawnMarkerTacticsTile_0", "asset", "Save SpawnMarkerTacticsTile_0", "Custom Assets/Tiles");
		// check that path exists in project
		if (path == "") {
			Debug.Log($"Path {path} is not available in Project to create a new SpawnMarkerTacticsTile_0 Instance");
			return;
		}

		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<SpawnMarkerTacticsTile_0>(), path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
#endif
}