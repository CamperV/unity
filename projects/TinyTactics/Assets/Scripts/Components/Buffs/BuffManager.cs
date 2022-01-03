using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    public HashSet<string> movementBuffProviders;

    void Awake() {
        movementBuffProviders = new HashSet<string>();
    }

    // public void AddDamageBuff(string provider, int damageValue, int expireTimerValue) {

    //     // first, check if we need to create a new buff or not
    //     if (HasBuffFromProvider<DamageBuff>(provider)) {
    //         DamageBuff existingBuff = GetBuffFromProvider<DamageBuff>(provider);

    //         existingBuff.AddValue(damageValue);
    //         existingBuff.TakeBestTimer(expireTimerValue);

    //     // else, if you're the first DamageBuff from this provider
    //     } else {
    //         DamageBuff buff = AttachBuff<DamageBuff>(provider);
    //         buff.buffValue = damageValue;
    //         buff.expireTimer = expireTimerValue;
    //     }
    // }

    // public void AddReflexDebuff(string provider, int debuffValue, int expireTimerValue) {

    //     // first, check if we need to create a new buff or not
    //     if (HasBuffFromProvider<ReflexDebuff>(provider)) {
    //         ReflexDebuff existingBuff = GetBuffFromProvider<ReflexDebuff>(provider);

    //         existingBuff.AddValue(debuffValue);
    //         existingBuff.TakeBestTimer(expireTimerValue);

    //     // else, if you're the first ReflexDebuff from this provider
    //     } else {
    //         ReflexDebuff buff = AttachBuff<ReflexDebuff>(provider);
    //         buff.SetValuesAndReapply(debuffValue, expireTimerValue);
    //     }
    // }

    public void AddValueBuff<T>(string provider, int buffValue, int expireTimerValue) where T : Buff {

        // first, check if we need to create a new buff or not
        if (HasBuffFromProvider<T>(provider)) {
            T existingBuff = GetBuffFromProvider<T>(provider);
            existingBuff.AddValuesAndReapply(buffValue, expireTimerValue);

        // else, if you're the first T from this provider
        } else {
            T buff = AttachBuff<T>(provider);
            buff.SetValuesAndReapply(buffValue, expireTimerValue);
        }
    }

    public void RemoveAllBuffsFromProvider(string provider) {
        foreach (Buff buff in GetComponents<Buff>()) {
            if (buff.provider == provider) Destroy(buff);
        }
    }

    public void RemoveAllMovementBuffs() {
        foreach (string mProvider in movementBuffProviders) {
            RemoveAllBuffsFromProvider(mProvider);
        }
    }

    //
    //
    //
    private T AttachBuff<T>(string provider) where T : Buff {
        return gameObject.AddComponent<T>().WithProvider(provider) as T;
    }

    private bool HasBuffFromProvider<T>(string provider) where T : Buff {
        bool hasBuff = false;

        foreach (T buff in GetComponents<T>()) {
            hasBuff |= buff.provider == provider;
        }
        return hasBuff;
    }

    private T GetBuffFromProvider<T>(string provider) where T : Buff {
        foreach (T buff in GetComponents<T>()) {
            if (buff.provider == provider) return buff;
        }

        // if you can't find a provider that matches, get the first Buff you see regardless
        // we should never get here
        return GetComponent<T>();
    }

    private void RemoveAllBuffs() {
        foreach (Buff buff in GetComponents<Buff>()) {
            Destroy(buff);
        }
    }
    private void RemoveAllBuffsOfType<T>() where T : Buff {
        foreach (T buff in GetComponents<T>()) {
            Destroy(buff);
        }
    }
}