using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GlossaryPanel : MonoBehaviour
{
	public EventManager eventManager;
	public TextMeshProUGUI glossaryText;

	public void OnEnable() {
		eventManager.DisablePlayerInput();
		UpdateActiveGlossaryText();
	}

	public void OnDisable() {
		eventManager.EnablePlayerInput();
	}

	private void UpdateActiveGlossaryText() {
		List<string> textLines = new List<string>();
		foreach (IToolTip tt in FindObjectsOfType<MonoBehaviour>().OfType<IToolTip>()) {
			textLines.Add($"<color=#FFDD70><b>{tt.tooltipName}</b></color>: {tt.tooltip}");
		}

		glossaryText.SetText( string.Join("\n\n", textLines.Distinct().OrderBy(it => it)) );
	}
}