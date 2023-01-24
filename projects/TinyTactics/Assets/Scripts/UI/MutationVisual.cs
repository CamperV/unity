using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Extensions;

public class MutationVisual : MonoBehaviour
{
	[SerializeField] private Image mainImage;
	[SerializeField] private TextMeshProUGUI text;

	public void SetInfo(Mutation mut) {
		mainImage.sprite = mut.sprite;
		text.SetText(mut.mutatorDisplayData.name);
	}
}
