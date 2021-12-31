using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    public string displayName { get; set; }

    public List<string> activeBuffs;

    void Awake() {
        activeBuffs = new List<string>();
    }

    public void AttachBuff<T>() where T : Buff {
        var existingBuff = GetComponent<T>();

        if (existingBuff != null) {
            existingBuff.Increment();

        // else, if you're the first buff of this type:
        } else {
            gameObject.AddComponent<T>();
        }

        activeBuffs.Add( typeof(T).ToString() );
    }
}