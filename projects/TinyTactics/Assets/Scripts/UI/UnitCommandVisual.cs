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
	[SerializeField] private TextMeshProUGUI commandName;
	[SerializeField] private GameObject activeBorder;
	[SerializeField] private GameObject activeCancelVisual;
	[SerializeField] private GameObject cooldown;
	[SerializeField] private GameObject remainingUses;
	[SerializeField] private TextMeshProUGUI slotNumber;

	// binds the appropriate UnitCommandSystem.IsCommandAvailable() call to refresh the buttons, without storing a reference explicitly
	private Func<bool> ButtonChecker;

	// binds UnitCommandSystem.LimitType things (cooldown, limiteduses) to refresh visuals
	private Action LimitTypeUpdater;

	public void OnActivate() {
		activeBorder.SetActive(true);
		// activeCancelVisual.SetActive(true);
	}
	public void OnDeactivate() {
		activeBorder.SetActive(false);
		// activeCancelVisual.SetActive(false);
	}

	public void SetImage(Sprite sprite) {
		mainImage.sprite = sprite;
	}

	public void SetName(string name) {
		commandName.SetText(name);
	}

	public void SetCooldown(int val) {
		if (val == 0) {
			cooldown.SetActive(false);
		} else {
			cooldown.SetActive(true);
			TextMeshProUGUI tmp = cooldown.GetComponentInChildren<TextMeshProUGUI>();
			tmp.SetText($"T-{val}");
		}	
	}

	public void SetRemainingUses(int val) {
		remainingUses.SetActive(true);
		TextMeshProUGUI tmp = remainingUses.GetComponentInChildren<TextMeshProUGUI>();
		tmp.SetText($"x{val}");
	}

	public void SetSlotNumber(int val) {
		slotNumber.SetText($"{val}");
	}

	public void RegisterCommand(UnityAction unityAction) {
		GetComponent<Button>().onClick.AddListener(unityAction);
	}

	public void SetButtonStatus(bool status) {
		GetComponent<Button>().interactable = status;
	}

	public void SetButtonChecker(Func<bool> _ButtonChecker) => ButtonChecker = _ButtonChecker;
	public void CheckButtonStatus() {
		GetComponent<Button>().interactable = ButtonChecker?.Invoke() ?? false;
	}

	public void SetLimitTypeUpdater(Action _LimitTypeUpdater) => LimitTypeUpdater = _LimitTypeUpdater;
	public void UpdateLimitType() {
		LimitTypeUpdater?.Invoke();
	}
}
