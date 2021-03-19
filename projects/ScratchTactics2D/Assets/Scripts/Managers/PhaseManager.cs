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

	private PlayerController playerControllerInst;
	private EnemyController enemyControllerInst;
	private Dictionary<Enum.Phase, string> phaseStringRepr = new Dictionary<Enum.Phase, string>() {
		[Enum.Phase.none] = "N/A",
		[Enum.Phase.player] = "Player",
		[Enum.Phase.enemy] = "Enemy"
	};
	
	void Awake() {
		currentPhaseByState = new Dictionary<Enum.GameState, Enum.Phase>() {
			[Enum.GameState.overworld] = Enum.Phase.player,
			[Enum.GameState.battle] = Enum.Phase.none
		};
		currentTurnByState = new Dictionary<Enum.GameState, int>() {
			[Enum.GameState.overworld] = 1,
			[Enum.GameState.battle] = 1
		};
	}
	
	// don't use Awake here, to avoid bootstrapping issues
    void Start() {
		playerControllerInst = GameManager.inst.playerController;
		enemyControllerInst = GameManager.inst.enemyController;

		currentPhase = Enum.Phase.player;
		currentTurn = 1;
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
					StartPhase(currentPhase.NextPhase());

					// if there's already a battle in progress, rejoin it
					var battle = GameManager.inst.tacticsManager.activeBattle;
					if (battle?.isPaused ?? false) {						
						battle.Resume();
						
						currentTurn++;
						StartPhase(Enum.Phase.player);
						battle.GetControllerFromPhase(Enum.Phase.player).TriggerPhase();

					// otherwise, give control back to the player
					} else {					
						playerControllerInst.TriggerPhase();
					}
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

		UIManager.inst.SetPhaseText(phaseStringRepr[phase]);
		UIManager.inst.SetTurnText(currentTurn.ToString());
	}
	
	public void OnPhaseEnd(Enum.Phase phase) {
		// every other Tactics-turn, we let the Overworld take a turn
		// here is where we tick overworldTurns while in the tactics interface
		if (GameManager.inst.gameState == Enum.GameState.battle) {
			if (phase == Enum.Phase.enemy && currentTurn % 2 == 0 && GameManager.inst.enemyController.enemiesFollowing) {
				Debug.Log($"Pausing and entering shadow overworld state");
				GameManager.inst.tacticsManager.activeBattle.Pause();

				GameManager.inst.enemyController.AddTicksAll(Constants.standardTickCost);
				StartPhase(Enum.Phase.enemy);
				enemyControllerInst.TriggerPhase();	/*
				// delay until activeBattle.isPaused becomes true
				StartCoroutine( Utils.DelayedFlag(!GameManager.inst.tacticsManager.activeBattle.isPaused, () => {
					// have every two turns equal standardTickCost ticks
					GameManager.inst.enemyController.AddTicksAll(Constants.standardTickCost);
					StartPhase(Enum.Phase.enemy);
					enemyControllerInst.TriggerPhase();
				}));*/
				return;
			}
		}

		// normal operation
		if (phase.NextPhase() == Enum.Phase.player)
			currentTurn++;
	}
	
	public void DisablePhase() {
		// don't set the UI text to display anything else
		currentPhase = Enum.Phase.none;
	}
}
