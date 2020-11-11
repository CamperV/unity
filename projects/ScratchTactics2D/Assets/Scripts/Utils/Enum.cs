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
	public enum EnemyState {idle, followField};
}
