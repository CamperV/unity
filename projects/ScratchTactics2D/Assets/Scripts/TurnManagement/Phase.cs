using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Extensions;

public class Phase
{
    public enum PhaseState {waitingForInput, complete};

    public delegate void Start();
    public delegate void End();
    public event Start StartEvent;
    public event End EndEvent;

    public string name;
    public PhaseState state;

    public Phase(string _name) {
        name = _name;
        state = PhaseState.waitingForInput;
    }

    public void TriggerStart() {
        state = PhaseState.waitingForInput;
        StartEvent.Invoke();
    }

    public void TriggerEnd() {
        state = PhaseState.complete;
        EndEvent.Invoke();
    }

    public override string ToString() {
        return $"Phase({name})";
    }
}