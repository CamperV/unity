using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Extensions;

public class UnitCommandVisual : MonoBehaviour
{
	[SerializeField] private Image mainImage;
	[SerializeField] private GameObject activeBorder;

	public void OnActivate() => activeBorder.SetActive(true);
	public void OnDeactivate() => activeBorder.SetActive(false);

	public void SetImage(Sprite sprite) {
		mainImage.sprite = sprite;
	}

	public void RegisterCommand(UnityAction unityAction) {
		GetComponent<Button>().onClick.AddListener(unityAction);
	}

	public void SetButtonStatus(bool status) {
		GetComponent<Button>().interactable = status;
	}
}
