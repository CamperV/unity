using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ArchetypeData : ScriptableObject
{
	private List<PerkData> _perkPoolCache = new List<PerkData>();

    // assign this in the inspector
    public string archetypeName;
    public Color color;

#if UNITY_EDITOR
    [MenuItem("Assets/Create/CustomAssets/ArchetypeData", false, 1)]
	private static void Create() {
		string path = EditorUtility.SaveFilePanelInProject("Save ArchetypeData", "ArchetypeData", "asset", "Save ArchetypeData", "Custom Assets/CustomData");
		// check that path exists in project
		if (path == "") {
			Debug.Log($"Path {path} is not available in Project to create a new ArchetypeData Instance");
			return;
		}

		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<ArchetypeData>(), path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
#endif

	public IEnumerable<PerkData> GetPerkPool() {
		if (_perkPoolCache.Count == 0) {
			foreach (PerkData loadedData in Resources.LoadAll<PerkData>("ScriptableObjects/PerkData")) {
				if (loadedData.belongsToArchetype == this && loadedData.isSignaturePerk == false) {
					_perkPoolCache.Add(loadedData);
				}
			}
		}

		foreach (PerkData perkData in _perkPoolCache) yield return perkData;
	}
}