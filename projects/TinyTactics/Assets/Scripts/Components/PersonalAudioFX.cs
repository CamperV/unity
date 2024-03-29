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
    public AudioFXBundle wakeUpAudioFXBundle;
    public AudioFXBundle avoidAudioFXBundle;
    public AudioFXBundle healAudioFXBundle;
    public AudioFXBundle deathAudioFXBundle;
    public AudioFXBundle breakAudioFXBundle;
    public AudioFXBundle criticalAudioFXBundle;
    public AudioFXBundle blockAudioFXBundle;
    public AudioFXBundle lethalAudioFXBundle;

    public bool IsPlaying() => audioSource.isPlaying;

    void Awake() {
        boundUnit = GetComponent<Unit>();
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayFX(AudioClip clip) => audioSource.PlayOneShot(clip, 1f);

    public void PlayInteractFX() {
        audioSource.PlayOneShot(interactAudioFXBundle.RandomClip(), 1f);
    }

    public void PlaySpecialInteractFX() {
        audioSource.PlayOneShot(specialInteractAudioFXBundle.RandomClip(), 1f);
    }

    public void PlayWakeUpFX() {
        audioSource.PlayOneShot(wakeUpAudioFXBundle.RandomClip(), 1f);
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

    public void PlayBreakFX() {
        audioSource.PlayOneShot(breakAudioFXBundle.RandomClip(), 1f);
    }

    public void PlayLethalDamageFX() {
        audioSource.PlayOneShot(lethalAudioFXBundle.RandomClip(), 1f);
    }

    // from the weapon itself
    public void PlayWeaponAttackFX() {
        audioSource.PlayOneShot(boundUnit.EquippedWeapon.audioFXBundle_Attack.RandomClip(), 1f);
    }

    public void PlayWeaponEquipFX() {
        audioSource.PlayOneShot(boundUnit.EquippedWeapon.audioFXBundle_Equip.RandomClip(), 1f);
    }
}
