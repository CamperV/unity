using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All global enum definitions here
namespace Enum {
	// full GameManager state
	public enum GameState {overworld, battle};
	
	// Phase management
	public enum Phase {none, player, enemy};
	public enum PhaseActionState {waitingForInput, acting, complete, postPhaseDelay, postPhase};
	//
	public enum EnemyState {idle, followField};
	public enum UnitState {idle};
}

public static class PhaseExtension {
	public static Enum.Phase NextPhase(this Enum.Phase p) {
		switch (p) {
			case Enum.Phase.player:
				return Enum.Phase.enemy;
			case Enum.Phase.enemy:
				return Enum.Phase.player;
			default:
				return Enum.Phase.none;
		}
	}
}
