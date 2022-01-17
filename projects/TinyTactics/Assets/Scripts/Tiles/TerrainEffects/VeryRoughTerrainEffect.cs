using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class VeryRoughTerrainEffect : TerrainEffect, IToolTip
{
    public string tooltipName { get; set; } = "Very Rough Terrain";
    public string tooltip { get; set; } = "+30 AVOID.";

    public override string shortDisplayName { get; set; } = "+30 AVO";

#if UNITY_EDITOR
    [MenuItem("Assets/Create/CustomAssets/VeryRoughTerrainEffect", false, 1)]
	private static void Create() {
		string path = EditorUtility.SaveFilePanelInProject("Save VeryRoughTerrainEffect", "VeryRoughTerrainEffect", "asset", "Save VeryRoughTerrainEffect", "Custom Assets/TerrainEffects");
		// check that path exists in project
		if (path == "") {
			Debug.Log($"Path {path} is not available in Project to create a new VeryRoughTerrainEffect Instance");
			return;
		}

		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<VeryRoughTerrainEffect>(), path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
#endif

    public override void OnEnterTerrain(Unit targetUnit) {
        if (!targetUnit.tags.Contains("Flier")) targetUnit.OnDefend += GrantAvoid;
    }

    public override void OnExitTerrain(Unit targetUnit) {
        if (!targetUnit.tags.Contains("Flier")) targetUnit.OnDefend -= GrantAvoid;
    }

    private void GrantAvoid(ref MutableDefense mutDef, Unit target) {
        mutDef.avoidRate += 30;
        //
        displayName = "Very Rough Terrain Bonus";
        mutDef.AddMutator(this);
    }
}