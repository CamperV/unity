using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class UIManager : MonoBehaviour
{
	public static UIManager inst = null; // enforces singleton behavior

    [SerializeField] private Text currentTurnText;
	[SerializeField] private Text currentPhaseText;
	
    void Awake() {
        // only allow one UIManager to exist at any time
		// & don't kill when reloading a Scene
 		if (inst == null) {
			inst = this;
		} else if (inst != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
    }

	public void UpdateTurn(int newTurn) {
		currentTurnText.text = $"Turn {newTurn}";
	}

	public void UpdatePhase(Phase newPhase) {
		currentPhaseText.text = $"Current Phase: {newPhase.name}"; 
	}
}