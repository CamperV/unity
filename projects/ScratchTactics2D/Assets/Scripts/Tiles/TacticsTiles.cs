using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

//-------------------------------------------------------------------//
public abstract class TacticsTile : Tile
{
	// returns an integer that signifies the cost of entering this tile
	public virtual int cost { get => 1; }

	//public virtual string spriteName { get; }
	//public List<Sprite> sprites;

	// public void OnEnable() {
	// 	sprites = new List<Sprite>() {
	// 		ResourceLoader.GetSprite($"Tiles/{spriteName}")
	// 	};
	// 	sprite = sprites[0];
	// }
}
//-------------------------------------------------------------------//

public class MountainTacticsTile : TacticsTile
{
	//public override string spriteName { get => "mountain_tile_iso_0"; }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/CustomAssets/MountainTacticsTile", false, 1)]
	private static void Create(){
		string path = EditorUtility.SaveFilePanelInProject("Save MountainTacticsTile", "MountainTacticsTile", "asset", "Save MountainTacticsTile", "Custom Assets/Tiles");
		// check that path exists in project
		if (path == "") {
			Debug.Log($"Path {path} is not available in Project to create a new MountainTacticsTile Instance");
			return;
		}

		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<MountainTacticsTile>(), path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
#endif
}