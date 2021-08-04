using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DockingMarkerTacticsTile_2 : TacticsTile
{
	//public override string spriteName { get => "docking_marker_iso_2"; }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/CustomAssets/DockingMarkerTacticsTile_2", false, 1)]
	private static void Create(){
		string path = EditorUtility.SaveFilePanelInProject("Save DockingMarkerTacticsTile_2", "DockingMarkerTacticsTile_2", "asset", "Save DockingMarkerTacticsTile_2", "Custom Assets/Tiles");
		// check that path exists in project
		if (path == "") {
			Debug.Log($"Path {path} is not available in Project to create a new DockingMarkerTacticsTile_2 Instance");
			return;
		}

		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<DockingMarkerTacticsTile_2>(), path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
#endif
}