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

	public void SetInfo(Mutation mut) {
		mainImage.sprite = mut.sprite;
	}
}
