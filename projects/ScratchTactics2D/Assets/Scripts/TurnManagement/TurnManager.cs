using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Extensions;

public class TurnManager : MonoBehaviour
{
    public Phase playerPhase;
    public Phase enemyPhase;

    [HideInInspector] public int turnCount = 0;
    [HideInInspector] public bool enable = true;
    private Coroutine loop;

    void Awake() {
        playerPhase = new Phase("Player");
        enemyPhase = new Phase("Enemy");

        Debug.Log($"TurnManager, {this.transform.parent} is awake");
    }

    // this will execute Enable() at the end of the Start frame
    void Start() {
        StartCoroutine( Utils.LateFrame(Enable) );
    }

    public void Enable() {
        Debug.Log($"Starting turn manager {this}, {transform.parent}");
        enable = true;
        StartCoroutine( _Loop() );
    }

    // by only touching the enable member, _Loop will terminate itself after the current Turn is over
    public void Disable() {
        Debug.Log($"Disabling turn manager {this}, {transform.parent}");
        enable = false;
    }

    public IEnumerator _Loop() {
        while (enable) {
            turnCount++;
            Debug.Log($"beginning {transform.parent} Turn {turnCount}");

            // create a new Turn, and wait until it has executed all of its phases
            Turn turn = new Turn(new List<Phase>{playerPhase, enemyPhase});
            yield return turn.ExecutePhases();
        }
    }
}