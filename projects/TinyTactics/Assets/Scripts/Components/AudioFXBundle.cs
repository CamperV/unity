using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AudioFXBundle : ScriptableObject
{
    // assign this in the inspector
    public List<AudioClip> clipPool;

#if UNITY_EDITOR
    [MenuItem("Assets/Create/CustomAssets/AudioFXBundle", false, 1)]
	private static void Create() {
		string path = EditorUtility.SaveFilePanelInProject("Save AudioFXBundle", "AudioFXBundle", "asset", "Save AudioFXBundle", "Custom Assets/AudioBundles");
		// check that path exists in project
		if (path == "") {
			Debug.Log($"Path {path} is not available in Project to create a new AudioFXBundle Instance");
			return;
		}

		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<AudioFXBundle>(), path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
#endif

    public AudioClip RandomClip() {
        return clipPool[ Random.Range(0, clipPool.Count) ];
    }
}