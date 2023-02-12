using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Extensions;

public class MutationVisual : MonoBehaviour, ITooltip
{
	[SerializeField] private Image mainImage;
	[SerializeField] private TextMeshProUGUI text;

	private string tooltip;

	public void SetInfo(Mutation mutation) {
		mainImage.sprite = mutation.sprite;
		text.SetText(mutation.mutatorDisplayData.name);

		tooltip = $"{mutation.mutatorDisplayData.name}".RichTextTags_TMP(bold: true);
		tooltip += "\n";
		tooltip += $"{mutation.mutatorDisplayData.description}".RichTextTags_TMP(italics: true);
	}

    // ITooltip
    public string GetTooltip() => tooltip;
}
