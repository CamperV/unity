using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Extensions;

public class PhaseManager : MonoBehaviour
{
	[HideInInspector] public Enum.Phase currentPhase { get; private set;	}
	[HideInInspector] public int currentTurn { get; private set; }
	[HideInInspector] public int currentPhaseCount { get; private set; }

	private PlayerController playerControllerInst;
	private EnemyController enemyControllerInst;
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
		playerControllerInst = GameManager.inst.playerController;
		enemyControllerInst = GameManager.inst.enemyController;
		
		GameManager.inst.UIManager.SetTurnText("Turn " + currentTurn);
    }

    void Update() {
		if (GameManager.inst.gameState == Enum.GameState.overworld) {
			// wait for phase objects to signal, and change phase for them
			if (currentPhase == Enum.Phase.player) {
				if (playerControllerInst.phaseActionState == Enum.PhaseActionState.postPhase) {
					OnPhaseEnd(currentPhase);
					StartPhase(Enum.Phase.enemy);
					enemyControllerInst.TriggerPhase();
				}
				//
				// code spins here until player takes its phaseAction
				//
			}
			else if (currentPhase == Enum.Phase.enemy) {
				if (enemyControllerInst.phaseActionState == Enum.PhaseActionState.postPhase) {
					OnPhaseEnd(currentPhase);
					StartPhase(Enum.Phase.player);
					playerControllerInst.TriggerPhase();
				}
				//
				// code spins here until all enemies takes their phaseAction
				//
			}
		} else if (GameManager.inst.gameState == Enum.GameState.battle) {
			var battle = GameManager.inst.tacticsManager.activeBattle;
			var activeController = battle.GetControllerFromPhase(currentPhase);

			// if the currently active controller has finished its phase
			if (activeController.phaseActionState == Enum.PhaseActionState.postPhase) {
				OnPhaseEnd(currentPhase);

				// after a phase has ended, use the controlled opportunity here
				// to potentially destroy the battle. Check if a side won
				if (!activeController.activeRegistry.Any()) {
					var defeated = battle.GetOverworldEntityFromPhase(currentPhase);
					GameManager.inst.tacticsManager.ResolveActiveBattle(defeated);
					return; // jump out of the update loop early
				}

				// else:
				StartPhase(currentPhase.NextPhase());
				battle.GetControllerFromPhase(currentPhase).TriggerPhase();
			}
		}
		// else, disable phasing
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
