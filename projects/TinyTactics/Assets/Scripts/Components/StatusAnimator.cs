using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Extensions;

[RequireComponent(typeof(StatusSystem))]
[RequireComponent(typeof(SpriteAnimator))]
[RequireComponent(typeof(MessageEmitter))]
[RequireComponent(typeof(AudioSource))]
public class StatusAnimator : MonoBehaviour
{
    private StatusSystem statusSystem;
    private SpriteAnimator spriteAnimator;
    private MessageEmitter messageEmitter;
    private AudioSource audioSource;

    [SerializeField] private AudioFXBundle genericBuffFXBundle;
    [SerializeField] private AudioFXBundle genericDebuffFXBundle;

    [SerializeField] private Color buffColor;
    [SerializeField] private Color debuffColor;
    
    void Awake() {
        statusSystem = GetComponent<StatusSystem>();
        spriteAnimator = GetComponent<SpriteAnimator>();
        messageEmitter = GetComponent<MessageEmitter>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start() {
        statusSystem.AddStatusEvent += AnimateAddStatus;
    }

    private void AnimateAddStatus(so_Status addedStatus) {
        if (addedStatus.hidden) return;

        // queue the sound and animation for after it is done animating other things
        switch(addedStatus.statusCode) {
            case so_Status.StatusCode.Buff:
                spriteAnimator.QueueAction(() => BuffAnimation(addedStatus));
                break;

            case so_Status.StatusCode.Debuff:
                spriteAnimator.QueueAction(() => DebuffAnimation(addedStatus));
                break;

            default:
                Debug.LogError($"Can't animate {addedStatus}");
                break;
        }
    }

    public void BuffAnimation(so_Status addedStatus) {
        audioSource.PlayOneShot(genericBuffFXBundle.RandomClip(), 1f);
        messageEmitter.Emit(MessageEmitter.MessageType.Buff, addedStatus.emitMessage);
        StartCoroutine( spriteAnimator.FlashColorThenRevert(buffColor) );
        StartCoroutine( spriteAnimator.SmoothCosX(32f, 0.015f, 0f, 1.0f) );
    }

    public void DebuffAnimation(so_Status addedStatus) {
        audioSource.PlayOneShot(genericDebuffFXBundle.RandomClip(), 1f);
        messageEmitter.Emit(MessageEmitter.MessageType.Debuff, addedStatus.emitMessage);
        StartCoroutine( spriteAnimator.FlashColorThenRevert(debuffColor) );
        StartCoroutine( spriteAnimator.SmoothCosX(18f, 0.03f, 0f, 1.0f) );
    }
}
