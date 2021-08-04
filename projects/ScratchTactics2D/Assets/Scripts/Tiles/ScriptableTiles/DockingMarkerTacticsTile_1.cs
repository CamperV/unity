using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DockingMarkerTacticsTile_1 : TacticsTile
{
	//public override string spriteName { get => "docking_marker_iso_1"; }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/CustomAssets/DockingMarkerTacticsTile_1", false, 1)]
	private static void Create(){
		string path = EditorUtility.SaveFilePanelInProject("Save DockingMarkerTacticsTile_1", "DockingMarkerTacticsTile_1", "asset", "Save DockingMarkerTacticsTile_1", "Custom Assets/Tiles");
		// check that path exists in project
		if (path == "") {
			Debug.Log($"Path {path} is not available in Project to create a new DockingMarkerTacticsTile_1 Instance");
			return;
		}

		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<DockingMarkerTacticsTile_1>(), path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
#endif
}