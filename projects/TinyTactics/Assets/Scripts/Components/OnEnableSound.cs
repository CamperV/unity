using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class OnEnableSound : MonoBehaviour
{
	private AudioSource audioSource;
	public AudioFXBundle onEnableFX;

    void Awake() {
		audioSource = GetComponent<AudioSource>();
	}

    void OnEnable() {
		audioSource.PlayOneShot(onEnableFX.RandomClip(), 1f);
    }
}