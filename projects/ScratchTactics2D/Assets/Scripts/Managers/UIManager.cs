using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	private Text currentPhaseText;
	private Text currentTurnText;
	
    void Awake() {
        currentPhaseText = GameObject.Find("CurrentPhaseText").GetComponent<Text>();
		    currentTurnText = GameObject.Find("CurrentTurnText").GetComponent<Text>();
    }

    public void SetPhaseText(string text) {
		  currentPhaseText.text = text;        
    }
	
    public void SetTurnText(string text) {
        currentTurnText.text = text;        
    }
}