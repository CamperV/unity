using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UnitBroadcastEventListener : MonoBehaviour
{
    // listening to:
    public UnitBroadcastEvent unitBroadcastEvent;

    // invoking when receiving broadcast event:
    public UnityEvent<Unit> OnEventRaised;

    void OnEnable() {
        // unitBroadcastEvent.BroadcastEvent.AddListener(EventRepeater);
        unitBroadcastEvent.AddListener(EventRepeater);
    }

    void OnDisable() {
        // unitBroadcastEvent.BroadcastEvent.RemoveListener(EventRepeater);
        unitBroadcastEvent.RemoveListener(EventRepeater);
    }

    private void EventRepeater(Unit unit) {
        OnEventRaised?.Invoke(unit);
    }
}
