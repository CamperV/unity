using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

// basically just a container to hold invocation references to happen OnWake, for things like Units
public class OnWakeHandler : MonoBehaviour
{
    public UnityEvent OnWake;
    public UnityEvent OnSleep;
}