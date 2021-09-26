using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class UIManager : MonoBehaviour
{
	// prefabs
	public UnitUI unitUIPrefab;

	// singleton
	public static UIManager inst = null; // enforces singleton behavior

	private Text currentOverworldPhaseText;
	private Text currentOverworldTurnText;
	private Text currentBattlePhaseText;
	private Text currentBattleTurnText;
	private Text currentFoodStoreText;
	private Text currentAccelerationToggleText;

	private GameObject overworldPhaseDisplay;
	private GameObject battlePhaseDisplay;
	private GameObject foodStoreDisplay;
	private GameObject accelerationToggleDisplay;
	
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

        currentOverworldPhaseText     = GameObject.Find("CurrentOverworldPhaseText").GetComponent<Text>();
		currentOverworldTurnText      = GameObject.Find("CurrentOverworldTurnText").GetComponent<Text>();
	    currentBattlePhaseText 	      = GameObject.Find("CurrentBattlePhaseText").GetComponent<Text>();
		currentBattleTurnText 	      = GameObject.Find("CurrentBattleTurnText").GetComponent<Text>();
		currentFoodStoreText          = GameObject.Find("CurrentFoodStoreText").GetComponent<Text>();
		currentAccelerationToggleText = GameObject.Find("AccelerationToggleText").GetComponent<Text>();

		overworldPhaseDisplay = GameObject.Find("OverworldPhaseDisplay");
		battlePhaseDisplay = GameObject.Find("BattlePhaseDisplay");
		foodStoreDisplay = GameObject.Find("FoodStoreDisplay");
		accelerationToggleDisplay = GameObject.Find("AccelerationToggleDisplay");
		EnableBattlePhaseDisplay(false);
    }

	void Start() {
		GameManager.inst.overworld.turnManager.playerPhase.StartEvent += UpdateOverworldDisplay;
		GameManager.inst.overworld.turnManager.enemyPhase.StartEvent += UpdateOverworldDisplay;

		GlobalPlayerState.FoodChangeEvent += UpdateFoodStoreDisplay;
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

	public void UpdateAccelerationToggleDisplay(bool? overrideBool = null) {
		bool currentToggle;
		if (overrideBool == null) {
			currentToggle = GameObject.Find("InputListener").GetComponent<InputListener>().toggle;
		} else {
			currentToggle = (bool)overrideBool;
		}
		string toggleChar = (currentToggle) ? "X" : "  ";
		currentAccelerationToggleText.text = $"Acceleration:  [{toggleChar}]"; 
	}

	public void CreateAndBindUnitUI(Unit boundUnit) {
		UnitUI unitUI = Instantiate(unitUIPrefab, boundUnit.transform);
		unitUI.BindUnit(boundUnit);

		// events
		boundUnit.UpdateHPEvent += unitUI.UpdateHealthBar;
		boundUnit.UpdateEquippedWeaponEvent += unitUI.UpdateWeaponEmblem;

		// unitUI.UpdateHealthBarThenFade(unitState._HP);

		// // suffering damage, or not
		// unitUI.DisplayDamageMessage(incomingDamage.ToString(), emphasize: isCritical);
		// unitUI.DisplayDamageMessage("MISS");

		// unitUI.SetTransparency(0.0f);
	}
}