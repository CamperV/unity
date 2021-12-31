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

    public void AddDamageBuff(string provider, int damageValue, int expireTimerValue) {

        // first, check if we need to create a new buff or not
        if (HasBuffFromProvider<DamageBuff>(provider)) {
            DamageBuff existingBuff = GetBuffFromProvider<DamageBuff>(provider);
            Debug.Log($"Found existing buff from provider {provider}: {existingBuff}");

            existingBuff.AddDamage(damageValue);
            existingBuff.TakeBestTimer(expireTimerValue);

        // else, if you're the first DamageBuff from this provider
        } else {
            DamageBuff buff = AttachBuff<DamageBuff>(provider);
            buff.buffDamage = damageValue;
            buff.expireTimer = expireTimerValue;
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