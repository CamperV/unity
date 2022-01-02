using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class HealTerrainEffect : TerrainEffect
{

#if UNITY_EDITOR
    [MenuItem("Assets/Create/CustomAssets/HealTerrainEffect", false, 1)]
	private static void Create() {
		string path = EditorUtility.SaveFilePanelInProject("Save HealTerrainEffect", "HealTerrainEffect", "asset", "Save HealTerrainEffect", "Custom Assets/TerrainEffects");
		// check that path exists in project
		if (path == "") {
			Debug.Log($"Path {path} is not available in Project to create a new HealTerrainEffect Instance");
			return;
		}

		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<HealTerrainEffect>(), path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
#endif

    public override void OnEnterTerrain(Unit targetUnit) {
        targetUnit.OnStartTurn += HealPercentage;
    }

    public override void OnExitTerrain(Unit targetUnit) {
        targetUnit.OnStartTurn -= HealPercentage;
    }

    private void HealPercentage(Unit target) {
        int healAmount = (int)(0.25f * target.unitStats.VITALITY);
        target.HealAmount(healAmount);
    }
}