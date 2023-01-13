using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

[CreateAssetMenu (menuName = "AudioFXBundle")]
public class AudioFXBundle : ScriptableObject
{
    // assign this in the inspector
    public List<AudioClip> clipPool;

    public AudioClip RandomClip() {
        return clipPool[ Random.Range(0, clipPool.Count) ];
    }
}