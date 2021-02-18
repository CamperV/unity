using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Extensions;

public class PhaseManager : MonoBehaviour
{
	private Dictionary<Enum.GameState, Enum.Phase> currentPhaseByState;
	[HideInInspector] public Enum.Phase currentPhase {
		get => currentPhaseByState[GameManager.inst.gameState];
		set => currentPhaseByState[GameManager.inst.gameState] = value;
	}

	private Dictionary<Enum.GameState, int> currentTurnByState;
	[HideInInspector] public int currentTurn {
		get => currentTurnByState[GameManager.inst.gameState];
		set => currentTurnByState[GameManager.inst.gameState] = value;
	}

	private Dictionary<Enum.GameState, int> currentPhaseCountByState;
	[HideInInspector] public int currentPhaseCount {
		get => currentPhaseCountByState[GameManager.inst.gameState];
		set => currentPhaseCountByState[GameManager.inst.gameState] = value;
	}

	private PlayerController playerControllerInst;
	private EnemyController enemyControllerInst;
	private Dictionary<Enum.Phase, string> phaseStringRepr = new Dictionary<Enum.Phase, string>() {
		[Enum.Phase.none] = "N/A",
		[Enum.Phase.player] = "Player",
		[Enum.Phase.enemy] = "Enemy"
	};
	
	void Awake() {
		currentPhaseByState = new Dictionary<Enum.GameState, Enum.Phase>() {
			[Enum.GameState.overworld] = Enum.Phase.none,
			[Enum.GameState.battle] = Enum.Phase.none
		};
		currentTurnByState = new Dictionary<Enum.GameState, int>() {
			[Enum.GameState.overworld] = 0,
			[Enum.GameState.battle] = 0
		};
		currentPhaseCountByState = new Dictionary<Enum.GameState, int>() {
			[Enum.GameState.overworld] = 0,
			[Enum.GameState.battle] = 0
		};

		currentPhase = Enum.Phase.none;
		currentPhaseCount = 0;
		currentTurn = 1;
	}
	
	// don't use Awake here, to avoid bootstrapping issues
    void Start() {
		playerControllerInst = GameManager.inst.playerController;
		enemyControllerInst = GameManager.inst.enemyController;
		
		UIManager.inst.SetTurnText("Turn " + currentTurn);
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

			// before doing anything, check flags to see if the battle should be terminated
			if (battle.CheckBattleEndState()) {
				GameManager.inst.tacticsManager.ResolveActiveBattle(battle.GetDefeated());
				return; // jump out of the update loop early
			}

			// if the currently active controller has finished its phase
			var activeController = battle.GetControllerFromPhase(currentPhase);
			if (activeController.phaseActionState == Enum.PhaseActionState.postPhase) {
				OnPhaseEnd(currentPhase);

				// else:
				StartPhase(currentPhase.NextPhase());
				battle.GetControllerFromPhase(currentPhase).TriggerPhase();
			}
		}
		// else, disable phasing
    }
	
	public void StartPhase(Enum.Phase phase) {
		currentPhase = phase;
		UIManager.inst.SetPhaseText("Current Phase: " + phaseStringRepr[currentPhase]);
		UIManager.inst.SetTurnText("Turn " + currentTurn);
	}
	
	public void OnPhaseEnd(Enum.Phase phase) {
		currentPhaseCount++;
		
		// update turn counter, start a new turn after two phases have completed
		if (currentPhaseCount % 2 == 0) {
			currentTurn++;
			UIManager.inst.SetTurnText("Turn " + currentTurn);
		}
	}
	
	public void DisablePhase() {
		// don't set the UI text to display anything else
		currentPhase = Enum.Phase.none;
	}
}
