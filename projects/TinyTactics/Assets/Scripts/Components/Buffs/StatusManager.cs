using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class StatusManager : MonoBehaviour
{
    public HashSet<string> movementBuffProviders;

    void Awake() {
        movementBuffProviders = new HashSet<string>();
    }

    public void AddValuedStatus<T>(string provider, int modifierValue) where T : ValuedStatus {

        // first, check if we need to create a new buff or not
        if (HasStatusFromProvider<T>(provider)) {
            T existingBuff = GetStatusFromProvider<T>(provider);
            existingBuff.AddValuesAndReapply(modifierValue);

        // else, if you're the first T from this provider
        } else {
            T buff = AttachStatus<T>(provider);
            buff.SetValuesAndReapply(modifierValue);
        }

        // TODO: there's a much better way to do all of this...
        //
        Unit thisUnit = GetComponent<Unit>();
        // T mutatorComp = GetComponent<T>();

        if (modifierValue > 0) {
            UIManager.inst.combatLog.AddEntry($"BLUE@[{provider}] applied a YELLOW@[+{modifierValue} {typeof(T).Name}] to {thisUnit.logTag}@[{thisUnit.displayName}].");
            // thisUnit.messageEmitter.Emit(MessageEmitter.MessageType.Buff, $"+{mutatorComp.affectedStat}");

        } else {
            UIManager.inst.combatLog.AddEntry($"BLUE@[{provider}] applied a YELLOW@[{modifierValue} {typeof(T).Name}] to {thisUnit.logTag}@[{thisUnit.displayName}].");
            // thisUnit.messageEmitter.Emit(MessageEmitter.MessageType.Debuff, $"-{mutatorComp.affectedStat}");
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