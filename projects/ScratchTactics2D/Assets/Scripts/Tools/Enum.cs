using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All global enum definitions here
namespace Enum {
	// full GameManager state
	public enum GameState {overworld, battle};
	
	// Phase management
	public enum Phase {none, player, enemy};
	public enum PhaseActionState {inactive, waitingForInput, acting, complete, postPhaseDelay, postPhase};
	//
	public enum EnemyArmyState {idle, followField, inBattle};
	
	// For armies on the overworld
	public enum VisibleState {visible, partiallyObscured, obscured, hidden};

	public enum PlayerUnitState {idle, menu, moveSelection, attackSelection};

	// Contextual Interaction states
	public enum InteractState {noSelection, unitSelected};
}