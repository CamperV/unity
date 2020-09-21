using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All global enum definitions here
namespace Enum {
	// Phase management
	public enum Phase {none, player, enemy};
	public enum PhaseActionState {waitingForInput, acting, complete, postPhaseDelay, postPhase};
	
	// overlay tile level
	public enum TileLevel {world, overlay, super};
}
