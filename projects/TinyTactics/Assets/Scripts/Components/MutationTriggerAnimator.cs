using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Extensions;

[RequireComponent(typeof(MutationSystem))]
[RequireComponent(typeof(SpriteAnimator))]
[RequireComponent(typeof(MessageEmitter))]
[RequireComponent(typeof(AudioSource))]
public class MutationTriggerAnimator : MonoBehaviour
{
    private MutationSystem mutationSystem;
    private SpriteAnimator spriteAnimator;
    private MessageEmitter messageEmitter;
    private AudioSource audioSource;

    [SerializeField] private AudioFXBundle triggerFXBundle;
    [SerializeField] private Color triggerColor;
    
    void Awake() {
        mutationSystem = GetComponent<MutationSystem>();
        spriteAnimator = GetComponent<SpriteAnimator>();
        messageEmitter = GetComponent<MessageEmitter>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start() {
        mutationSystem.MutationProcEvent += AnimateTrigger;
    }

    private void AnimateTrigger(Mutation triggeredMutation) {
        // queue the sound and animation for after it is done animating other things
        spriteAnimator.QueueAction(() => TriggerAnimation(triggeredMutation));
    }

    public void TriggerAnimation(Mutation triggeredMutation) {
        audioSource.PlayOneShot(triggerFXBundle.RandomClip(), 1f);
        messageEmitter.Emit(MessageEmitter.MessageType.Buff, triggeredMutation.mutatorDisplayData.name);
        StartCoroutine( spriteAnimator.FlashColorThenRevert(triggerColor) );
        StartCoroutine( spriteAnimator.SmoothCosX(32f, 0.015f, 0f, 1.0f) );
    }
}
