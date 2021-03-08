﻿using System.Collections;
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

	private GameObject overworldPhaseDisplay;
	private GameObject battlePhaseDisplay;
	
    void Awake() {
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

		overworldPhaseDisplay = GameObject.Find("OverworldPhaseDisplay");
		battlePhaseDisplay = GameObject.Find("BattlePhaseDisplay");
		EnableBattlePhaseDisplay(false);
    }

    public void SetPhaseText(string text) {
		if (GameManager.inst.gameState == Enum.GameState.overworld)
			currentOverworldPhaseText.text = $"Current Overworld Phase: {text}";  
		else if (GameManager.inst.gameState == Enum.GameState.battle)
			currentBattlePhaseText.text = $"Current Battle Phase: {text}";
    }
	
    public void SetTurnText(string text) {
		if (GameManager.inst.gameState == Enum.GameState.overworld)
			currentOverworldTurnText.text = $"Turn {text}";  
		else if (GameManager.inst.gameState == Enum.GameState.battle)
			currentBattleTurnText.text = $"Turn {text}";  
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