using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalPlayerState : MonoBehaviour
{
    // singleton
	public static GlobalPlayerState inst = null; // enforces singleton behavior

    public static PlayerArmy army { get => GameManager.inst.playerArmy; }
    public static PlayerArmyController controller { get => GameManager.inst.playerArmyController; }

    public static HashSet<Vector3Int> previouslyRevealedOverworldPositions = new HashSet<Vector3Int>();

	void Awake() {
		// only allow one GlobalPlayerState to exist at any time
		// & don't kill when reloading a Scene
 		if (inst == null) {
			inst = this;
		} else if (inst != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);

        previouslyRevealedOverworldPositions = new HashSet<Vector3Int>();
	}
}
