using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseManager : MonoBehaviour
{
	[HideInInspector] public Enum.Phase currentPhase { get; private set;	}
	[HideInInspector] public int currentTurn { get; private set; }
	[HideInInspector] public int currentPhaseCount { get; private set; }

	private Player playerInst;
	private EnemyManager enemyManagerInst;
	private Dictionary<Enum.Phase, string> phaseStringRepr = new Dictionary<Enum.Phase, string>() {
		[Enum.Phase.none] = "N/A",
		[Enum.Phase.player] = "Player",
		[Enum.Phase.enemy] = "Enemy"
	};
	
	void Awake() {
		currentPhase = Enum.Phase.none;
		currentPhaseCount = 0;
		currentTurn = 1;
	}
	
	// dont' use Awake here, to avoid bootstrapping issues
    void Start() {
		playerInst = GameManager.inst.player;
		enemyManagerInst = GameManager.inst.enemyManager;
		
		GameManager.inst.UIManager.SetTurnText("Turn " + currentTurn);
    }

    void Update() {
        // wait for phase objects to signal, and change phase for them
		if (currentPhase == Enum.Phase.player) {
			if (playerInst.phaseActionState == Enum.PhaseActionState.postPhase) {
				OnPhaseEnd(currentPhase);
				StartPhase(Enum.Phase.enemy);
				enemyManagerInst.TriggerPhase();
			}
			//
			// code spins here until player takes its phaseAction
			//
		}
		else if (currentPhase == Enum.Phase.enemy) {
			if (enemyManagerInst.phaseActionState == Enum.PhaseActionState.postPhase) {
				OnPhaseEnd(currentPhase);
				StartPhase(Enum.Phase.player);
				playerInst.TriggerPhase();
			}
			//
			// code spins here until all enemies takes their phaseAction
			//
		}
    }
	
	public void StartPhase(Enum.Phase phase) {
		currentPhase = phase;
		GameManager.inst.UIManager.SetPhaseText("Current Phase: " + phaseStringRepr[currentPhase]);
	}
	
	public void OnPhaseEnd(Enum.Phase phase) {
		currentPhaseCount++;
		
		// update turn counter, start a new turn after two phases have completed
		if (currentPhaseCount % 2 == 0) {
			currentTurn++;
			GameManager.inst.UIManager.SetTurnText("Turn " + currentTurn);
		}
	}
	
	public void DisablePhase() {
		// don't set the UI text to display anything else
		currentPhase = Enum.Phase.none;
	}
}
