using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Extensions;

public class TurnManager : MonoBehaviour
{
    public Phase playerPhase;
    public Phase enemyPhase;
    public Phase currentPhase;

    [HideInInspector] public int turnCount = 0;
    [HideInInspector] public bool enable = true;
    [HideInInspector] public bool suspend = false;

    void Awake() {
        playerPhase = new Phase("Player");
        enemyPhase = new Phase("Enemy");
    }

    // this will execute Enable() at the end of the Start frame
    void Start() {
        StartCoroutine( Utils.LateFrame(Enable) );
    }

    public void Enable() {
        Debug.Log($"Starting turn manager {gameObject.name}");
        enable = true;
        StartCoroutine( Loop() );
    }

    // by only touching the enable member, Loop will terminate itself after the current Turn is over
    public void Disable() {
        Debug.Log($"Disabling turn manager {gameObject.name}");
        enable = false;
    }

    public void Suspend() {
        Debug.Log($"Suspending turn manager {gameObject.name}");
        suspend = true;
    }

    public void Resume() {
        Debug.Log($"Resuming turn manager {gameObject.name}");
        suspend = false;
    }

    private IEnumerator Loop() {
        while (enable) {
            turnCount++;
            Debug.Log($"beginning {gameObject.name} Turn {turnCount}");
            yield return ExecutePhases(playerPhase, enemyPhase);
        }
    }

    // start each phase in order, and wait until finished before starting the next
    private IEnumerator ExecutePhases(params Phase[] phases) {
        foreach (Phase phase in phases) {
            // first, check the suspension signal
            // this is different than disabling, which lets all phases play out
            // this will suspend and allow resumption
            if (suspend) {
                yield return new WaitUntil(() => suspend == false);
            }

            Debug.Log($"Triggering phase {phase.name}");
            currentPhase = phase;
            phase.TriggerStart();
            yield return new WaitUntil(() => phase.state == Phase.PhaseState.complete);
        }
    }
}