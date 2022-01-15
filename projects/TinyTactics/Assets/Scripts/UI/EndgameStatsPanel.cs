using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndgameStatsPanel : MonoBehaviour
{
	public TextMeshProUGUI enemiesDefeatedValue;
	public TextMeshProUGUI survivingUnitsValue;
	public TextMeshProUGUI turnsElapsedValue;
	
	public Button mainMenuButton;
	public Button continueButton;

	void OnEnable() {
		if (Campaign.active == null) {
			continueButton.gameObject.SetActive(false);
		}
	}
}