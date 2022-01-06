using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GlossaryPanel : MonoBehaviour
{
	public TextMeshProUGUI glossaryText;

	public void SetActiveGlossaryText() {
		List<string> textLines = new List<string>();

		foreach (IToolTip tt in FindObjectsOfType<MonoBehaviour>().OfType<IToolTip>()) {
			textLines.Add($"<b>{tt.tooltipName}</b>: {tt.tooltip}");
		}

		glossaryText.SetText( string.Join("\n\n", textLines.Distinct().OrderBy(it => it)) );
	}

	public void Update() {
		SetActiveGlossaryText();
	}
}