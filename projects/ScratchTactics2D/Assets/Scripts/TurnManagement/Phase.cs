using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Extensions;

public class Phase
{
    public enum PhaseState {inactive, active, complete};

    public delegate void Start();
    public delegate void End();
    public event Start StartEvent;
    public event End EndEvent;

    public string name;
    public PhaseState state;

    public Phase(string _name) {
        name = _name;
        state = PhaseState.inactive;
    }

    public void TriggerStart() {
        state = PhaseState.active;
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