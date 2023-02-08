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

		tooltip = mutation.mutatorDisplayData.description;
		Debug.Log($"Set tooltip to {tooltip}");
	}

    // ITooltip
    public string GetTooltip() => tooltip;
}
