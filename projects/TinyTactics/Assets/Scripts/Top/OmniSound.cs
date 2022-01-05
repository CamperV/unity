using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OmniSound : MonoBehaviour
{
    public static OmniSound inst = null;

    [HideInInspector] public AudioSource audioSource;
    public AudioClip clickFX;
    public AudioClip confirmFX;

    void Awake() {
        audioSource = GetComponent<AudioSource>();

        // only allow one OmniSound to exist at any time
        // & don't kill when reloading a Scene
        if (inst == null) {
            inst = this;
        } else if (inst != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public static void PlayClickFX() => OmniSound.inst._PlayFX(OmniSound.inst.clickFX, 1.0f);
    public static void PlayConfirmFX() => OmniSound.inst._PlayFX(OmniSound.inst.confirmFX, 1.0f);

    private void _PlayFX(AudioClip fx, float vol) => audioSource.PlayOneShot(fx, vol);
}