using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OmniSound : MonoBehaviour
{
    public static OmniSound inst = null;

    [HideInInspector] public AudioSource audioSource;
    public AudioClip clickFX;

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

    public void PlayClickFX() => audioSource.PlayOneShot(clickFX, 1.0f);
}