using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Extensions;

public class Turn
{
    protected List<Phase> phases;

    public Turn(List<Phase> _phases) {
        phases = _phases;
    }

    // start each phase in order, and wait until finished before starting the next
    public IEnumerator ExecutePhases() {
        foreach (Phase phase in phases) {
            Debug.Log($"Triggering phase {phase.name}");
		    UIManager.inst.SetPhaseText(phase.name);
            
            phase.TriggerStart();
            yield return new WaitUntil(() => phase.state == Phase.PhaseState.complete);
        }
    }
}