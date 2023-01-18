using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Extensions;

public class Phase
{
    public enum PhaseState {
        Inactive,
        Active,
        Complete
    };
    public PhaseState state;

    public delegate void Start();
    public delegate void End();
    public event Start StartEvent;
    public event End EndEvent;

    public string name;

    public Phase(string _name) {
        name = _name;
        state = PhaseState.Inactive;
    }

    public void TriggerStart() {
        state = PhaseState.Active;
        StartEvent?.Invoke();
    }

    public void TriggerEnd() {
        state = PhaseState.Complete;
        EndEvent?.Invoke();
    }

    public override string ToString() {
        return $"Phase({name})";
    }
}