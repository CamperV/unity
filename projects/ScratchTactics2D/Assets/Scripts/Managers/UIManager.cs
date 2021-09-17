using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	// singleton
	public static UIManager inst = null; // enforces singleton behavior

	private Text currentOverworldPhaseText;
	private Text currentOverworldTurnText;
	private Text currentBattlePhaseText;
	private Text currentBattleTurnText;
	private Text currentFoodStoreText;

	private GameObject overworldPhaseDisplay;
	private GameObject battlePhaseDisplay;
	private GameObject foodStoreDisplay;
	
    void Awake() {
		Debug.Log($"UI awake");
        // only allow one UIManager to exist at any time
		// & don't kill when reloading a Scene
 		if (inst == null) {
			inst = this;
		} else if (inst != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);

        currentOverworldPhaseText = GameObject.Find("CurrentOverworldPhaseText").GetComponent<Text>();
		currentOverworldTurnText  = GameObject.Find("CurrentOverworldTurnText").GetComponent<Text>();
	    currentBattlePhaseText 	  = GameObject.Find("CurrentBattlePhaseText").GetComponent<Text>();
		currentBattleTurnText 	  = GameObject.Find("CurrentBattleTurnText").GetComponent<Text>();
		currentFoodStoreText      = GameObject.Find("CurrentFoodStoreText").GetComponent<Text>();

		overworldPhaseDisplay = GameObject.Find("OverworldPhaseDisplay");
		battlePhaseDisplay = GameObject.Find("BattlePhaseDisplay");
		foodStoreDisplay = GameObject.Find("FoodStoreDisplay");
		EnableBattlePhaseDisplay(false);
    }

	void Start() {
		GameManager.inst.overworld.turnManager.playerPhase.StartEvent += UpdateOverworldDisplay;
		GameManager.inst.overworld.turnManager.enemyPhase.StartEvent += UpdateOverworldDisplay;

		GlobalPlayerState.FoodChangeEvent += UpdateFoodStoreDisplay;

		// below is moved to Battle.Start(), because otherwise it is null
		// Battle.active.turnManager.playerPhase.StartEvent += UpdateBattleDisplay;
		// Battle.active.turnManager.enemyPhase.StartEvent += UpdateBattleDisplay;
	}

	private void UpdateOverworldDisplay() {
		TurnManager tm = GameManager.inst.overworld.turnManager;
		currentOverworldPhaseText.text = $"Current Overworld Phase: {tm.currentPhase.name}"; 
		currentOverworldTurnText.text = $"Turn {tm.turnCount}";
	}

	public void UpdateBattleDisplay() {
		TurnManager tm = Battle.active.turnManager;
		currentBattlePhaseText.text = $"Current Battle Phase: {tm.currentPhase.name}"; 
		currentBattleTurnText.text = $"Turn {tm.turnCount}";
	}

	private void UpdateFoodStoreDisplay() {
		currentFoodStoreText.text = $"Food Store: {GlobalPlayerState.currentFoodStore}"; 
	}

	public void EnableBattlePhaseDisplay(bool enable) {
		battlePhaseDisplay.SetActive(enable);

		if (enable) {
			overworldPhaseDisplay.transform.localScale = 0.85f * Vector3.one;
			battlePhaseDisplay.transform.localScale = Vector3.one;
		} else {
			overworldPhaseDisplay.transform.localScale = Vector3.one;
			battlePhaseDisplay.transform.localScale = 0.85f * Vector3.one;
		}
	}
}