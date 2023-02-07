using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[CreateAssetMenu (menuName = "Events/UnitBroadcastEvent")]
public class UnitBroadcastEvent : ScriptableObject
{
    private UnityEvent<Unit> @event;

    public void AddListener(UnityAction<Unit> listener) {
        @event.AddListener(listener);
    }

    public void RemoveListener(UnityAction<Unit> listener) {
        @event.RemoveListener(listener);
    }

    public void Invoke(Unit unit) {
        @event?.Invoke(unit);
    }
}
