using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ForestTreeTacticsTile : TacticsTile
{
	public override int cost { get => -1; }
	public override bool dimmable { get => true; }
	public override float zHeight { get => 3; }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/CustomAssets/ForestTreeTacticsTile", false, 1)]
	private static void Create(){
		string path = EditorUtility.SaveFilePanelInProject("Save ForestTreeTacticsTile", "ForestTreeTacticsTile", "asset", "Save ForestTreeTacticsTile", "Custom Assets/Tiles");
		// check that path exists in project
		if (path == "") {
			Debug.Log($"Path {path} is not available in Project to create a new ForestTreeTacticsTile Instance");
			return;
		}

		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<ForestTreeTacticsTile>(), path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
#endif
}