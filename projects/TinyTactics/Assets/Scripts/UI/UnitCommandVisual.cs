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

	// can assign in inspector, for things like AttackUC's WeaponSwitcher
	public UnityEvent<PlayerUnit, UnitCommand> PropagateActivation;
	public UnityEvent<PlayerUnit, UnitCommand> PropagateDeactivation;
	//

	public void OnActivate(PlayerUnit thisUnit, UnitCommand thisCommand) {
		activeBorder.SetActive(true);
		PropagateActivation?.Invoke(thisUnit, thisCommand);
	}
	public void OnDeactivate(PlayerUnit thisUnit, UnitCommand thisCommand) {
		activeBorder.SetActive(false);
		PropagateDeactivation?.Invoke(thisUnit, thisCommand);
	}

	public void SetCommandInfo(UnitCommand uc) {
		mainImage.sprite = uc.sprite;
		commandName.SetText(uc.name);
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
