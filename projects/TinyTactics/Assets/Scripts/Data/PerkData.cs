using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PerkData : ScriptableObject
{
    // assign this in the inspector
    public string perkName;
	public string typeName;
	public string description;
	//
	public ArchetypeData belongsToArchetype;

#if UNITY_EDITOR
    [MenuItem("Assets/Create/CustomAssets/PerkData", false, 1)]
	private static void Create() {
		string path = EditorUtility.SaveFilePanelInProject("Save PerkData", "PerkData", "asset", "Save PerkData", "Custom Assets/CustomData");
		// check that path exists in project
		if (path == "") {
			Debug.Log($"Path {path} is not available in Project to create a new PerkData Instance");
			return;
		}

		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<PerkData>(), path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
#endif 
}