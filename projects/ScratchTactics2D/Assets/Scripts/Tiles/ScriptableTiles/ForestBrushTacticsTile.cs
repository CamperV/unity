using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ForestBrushTacticsTile : TacticsTile
{
	public override int cost { get => 2; }
	public override bool dimmable { get => true; }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/CustomAssets/ForestBrushTacticsTile", false, 1)]
	private static void Create(){
		string path = EditorUtility.SaveFilePanelInProject("Save ForestBrushTacticsTile", "ForestBrushTacticsTile", "asset", "Save ForestBrushTacticsTile", "Custom Assets/Tiles");
		// check that path exists in project
		if (path == "") {
			Debug.Log($"Path {path} is not available in Project to create a new ForestBrushTacticsTile Instance");
			return;
		}

		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<ForestBrushTacticsTile>(), path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
#endif
}