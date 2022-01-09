using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class StatusManager : MonoBehaviour
{
    public AudioFXBundle buffAudioFXBundle;
    public AudioFXBundle debuffAudioFXBundle;

    public HashSet<string> movementBuffProviders;

    void Awake() {
        movementBuffProviders = new HashSet<string>();
    }

    public void AddValuedStatus<T>(string provider, int modifierValue, int expireTimerValue) where T : ValuedStatus {

        // first, check if we need to create a new buff or not
        if (HasStatusFromProvider<T>(provider)) {
            T existingBuff = GetStatusFromProvider<T>(provider);
            existingBuff.AddValuesAndReapply(modifierValue, expireTimerValue);

        // else, if you're the first T from this provider
        } else {
            T buff = AttachStatus<T>(provider);
            buff.SetValuesAndReapply(modifierValue, expireTimerValue);
        }

        // the just-added status
        T valuedStatus = GetStatusFromProvider<T>(provider);
        if (typeof(T).IsSubclassOf(typeof(Buff))) {
            GetComponent<AudioSource>().PlayOneShot(buffAudioFXBundle.RandomClip(), 0.5f);

        } else if (typeof(T).IsSubclassOf(typeof(Debuff))) {
            GetComponent<AudioSource>().PlayOneShot(debuffAudioFXBundle.RandomClip(), 0.5f);
        }
    }

    public void AddConditionalBuff<T>(string provider, int modifierValue, Func<bool> Condition) where T : ConditionalBuff {
        // don't worry about stacking, just keep adding Components.
        T buff = AttachStatus<T>(provider);
        buff.ApplyValueAndCondition(modifierValue, Condition);
    }

    public void RemoveAllStatusFromProvider(string provider) {
        foreach (Status s in GetComponents<Status>()) {
            if (s.provider == provider) Destroy(s);
        }
    }

    public void RemoveAllMovementBuffs() {
        foreach (string mProvider in movementBuffProviders) {
            RemoveAllStatusFromProvider(mProvider);
        }
    }

    //
    //
    //
    private T AttachStatus<T>(string provider) where T : Status {
        return gameObject.AddComponent<T>().WithProvider(provider) as T;
    }

    public bool HasStatusFromProvider<T>(string provider) where T : Status {
        bool hasStatus = false;

        foreach (T t in GetComponents<T>()) {
            hasStatus |= t.provider == provider;
        }
        return hasStatus;
    }

    private T GetStatusFromProvider<T>(string provider) where T : Status {
        foreach (T s in GetComponents<T>()) {
            if (s.provider == provider) return s;
        }

        // if you can't find a provider that matches, get the first Buff you see regardless
        // we should never get here
        return GetComponent<T>();
    }
}