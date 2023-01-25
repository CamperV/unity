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
    public UnityEvent<Unit> BroadcastEvent;
}
