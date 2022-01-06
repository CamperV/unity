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
		// for all IToolTip MBs
		List<IToolTip> toolTippers = FindObjectsOfType<MonoBehaviour>().OfType<IToolTip>().ToList();

		// for all IToolTip SOs
		// TODO WOW this sucks
		toolTippers.Add(ScriptableObject.CreateInstance<HealTerrainEffect>());
		toolTippers.Add(ScriptableObject.CreateInstance<RoughTerrainEffect>());
		
		List<string> textLines = new List<string>();

		foreach (IToolTip tt in toolTippers) {
			textLines.Add($"<color=#FFDD70><b>{tt.tooltipName}</b></color>: {tt.tooltip}");
		}

		glossaryText.SetText( string.Join("\n\n", textLines.Distinct().OrderBy(it => it)) );
	}
}