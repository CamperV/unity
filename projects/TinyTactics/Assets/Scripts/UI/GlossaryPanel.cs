using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GlossaryPanel : MonoBehaviour
{
	public TextMeshProUGUI glossaryText;

	public void OnEnable() {
		UpdateActiveGlossaryText();
	}

	private void UpdateActiveGlossaryText() {
		// for all ITooltip MBs
		List<ITooltip> toolTippers = FindObjectsOfType<MonoBehaviour>().OfType<ITooltip>().ToList();
		
		List<string> textLines = new List<string>();

		foreach (ITooltip tt in toolTippers) {
			textLines.Add($"<color=#FFDD70><b>n/a</b></color>: {tt.GetTooltip()}");
		}

		glossaryText.SetText( string.Join("\n\n", textLines.Distinct().OrderBy(it => it)) );
	}
}