using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BattleJukeBox : MonoBehaviour
{
    // configured to:
    // - loop
    // - not play on awake
    private AudioSource audioSource;

    [SerializeField] private AudioClip prebattleTrack;
    [SerializeField] private AudioClip battleTrack;
    [SerializeField] private AudioClip victoryTrack;
    [SerializeField] private AudioClip defeatTrack;


    // audioSource will be configured to loop
    // use this guy to transition between 3 tracks
    // pre-battle
    // battle
    // victory or defeat
    void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    void Start() {
        audioSource.volume = 0.5f;
        audioSource.clip = prebattleTrack;
        audioSource.Play();
    }

    public void SwitchToPreBattleTrack(float inTime) => StartCoroutine( FadeOutAndPlay(inTime, prebattleTrack) );
    public void SwitchToBattleTrack(float inTime) =>    StartCoroutine( FadeOutAndIn(0.25f, inTime, battleTrack, 0.10f) );
    public void SwitchToVictoryTrack(float inTime) =>   StartCoroutine( FadeOutAndIn(0.25f, inTime, victoryTrack, 0.10f) );
    public void SwitchToDefeatTrack(float inTime) =>    StartCoroutine( FadeOutAndIn(0.25f, inTime, defeatTrack, 0.10f) );

    private IEnumerator FadeOutAndPlay(float fadeTime, AudioClip newClip) {
        float timeRatio = 0f;
        float startingVolume = audioSource.volume;

        // fade down here
        while (timeRatio < 1.0f) {
            timeRatio += (Time.deltaTime / fadeTime);
            audioSource.volume = startingVolume * (1f - timeRatio);
            yield return null;
        }
        
        // then play here
        audioSource.clip = newClip;
        audioSource.volume = startingVolume;
		audioSource.Play();
    }

    private IEnumerator FadeOutAndIn(float fadeOutTime, float fadeInTime, AudioClip newClip, float atVolume) {
        float timeRatio = 0f;
        float startingVolume = audioSource.volume;

        // fade down here
        while (timeRatio < 1.0f) {
            timeRatio += (Time.deltaTime / fadeOutTime);
            audioSource.volume = startingVolume * (1f - timeRatio);
            yield return null;
        }
        
        // then play here
        audioSource.clip = newClip;
		audioSource.Play();

        timeRatio = 0f;
        while (timeRatio < 1.0f) {
            timeRatio += (Time.deltaTime / fadeInTime);
            audioSource.volume = atVolume * timeRatio;
            yield return null;
        }

        audioSource.volume = atVolume;
    }
}
