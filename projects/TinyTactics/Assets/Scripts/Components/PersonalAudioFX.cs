using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class PersonalAudioFX : MonoBehaviour
{
    [HideInInspector] public Unit boundUnit;
    [HideInInspector] public AudioSource audioSource;

    public AudioFXBundle interactAudioFXBundle;
    public AudioFXBundle specialInteractAudioFXBundle;
    [HideInInspector] public AudioFXBundle weaponAudioFXBundle; // hidden because it comes from the boundUnit's weapon
    public AudioFXBundle avoidAudioFXBundle;
    public AudioFXBundle healAudioFXBundle;
    public AudioFXBundle deathAudioFXBundle;
    public AudioFXBundle criticalAudioFXBundle;
    public AudioFXBundle blockAudioFXBundle;

    void Awake() {
        boundUnit = GetComponent<Unit>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start() {
        weaponAudioFXBundle = boundUnit.equippedWeapon.audioFXBundle;
    }

    public void PlayFX(AudioClip clip) => audioSource.PlayOneShot(clip, 1f);

    public void PlayInteractFX() {
        audioSource.PlayOneShot(interactAudioFXBundle.RandomClip(), 1f);
    }

    public void PlaySpecialInteractFX() {
        audioSource.PlayOneShot(specialInteractAudioFXBundle.RandomClip(), 1f);
    }

    public void PlayWeaponAttackFX() {
        audioSource.PlayOneShot(weaponAudioFXBundle.RandomClip(), 1f);
    }

    public void PlayAvoidFX() {
        audioSource.PlayOneShot(avoidAudioFXBundle.RandomClip(), 1f);
    }

    public void PlayHealFX() {
        audioSource.PlayOneShot(healAudioFXBundle.RandomClip(), 1f);
    }

    public void PlayDeathFX() {
        audioSource.PlayOneShot(deathAudioFXBundle.RandomClip(), 1f);
    }

    public void PlayCriticalFX() {
        audioSource.PlayOneShot(criticalAudioFXBundle.RandomClip(), 1f);
    }

    public void PlayBlockFX() {
        audioSource.PlayOneShot(blockAudioFXBundle.RandomClip(), 1f);
    }
}
