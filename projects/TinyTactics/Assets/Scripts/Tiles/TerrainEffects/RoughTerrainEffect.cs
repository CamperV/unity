using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RoughTerrainEffect : TerrainEffect
{

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
        Debug.Log($"{targetUnit} entered Rough Tile");
        targetUnit.OnDefend += GrantAvoid;
    }

    public override void OnExitTerrain(Unit targetUnit) {
        Debug.Log($"{targetUnit} left Rough Tile");
        targetUnit.OnDefend -= GrantAvoid;
    }

    private void GrantAvoid(ref MutableDefense mutDef, Unit target) {
        mutDef.avoidRate += 15;
        //
        displayName = "Rough Terrain Bonus";
        mutDef.AddMutator(this);
    }
}