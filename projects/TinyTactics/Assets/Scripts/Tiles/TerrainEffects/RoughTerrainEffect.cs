using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RoughTerrainEffect : TerrainEffect, IToolTip
{
    public string tooltipName { get; set; } = "Rough Terrain";
    public string tooltip { get; set; } = "+15 AVOID.";

    public override string shortDisplayName { get; set; } = "+15 AVO";

#if UNITY_EDITOR
    [MenuItem("Assets/Create/CustomAssets/RoughTerrainEffect", false, 1)]
	private static void Create() {
		string path = EditorUtility.SaveFilePanelInProject("Save RoughTerrainEffect", "RoughTerrainEffect", "asset", "Save RoughTerrainEffect", "Custom Assets/TerrainEffects");
		// check that path exists in project
		if (path == "") {
			Debug.Log($"Path {path} is not available in Project to create a new RoughTerrainEffect Instance");
			return;
		}

		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<RoughTerrainEffect>(), path);
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

    private void GrantAvoid(Unit thisUnit, ref MutableDefense mutDef, Unit target) {
        // mutDef.avoidRate += 15;
        //
        displayName = "Rough Terrain Bonus (does nothing)";
        mutDef.AddMutator(this);
    }
}