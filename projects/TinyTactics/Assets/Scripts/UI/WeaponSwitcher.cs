using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Extensions;

public class WeaponSwitcher : MonoBehaviour
{
	private PlayerUnit boundUnit;
	private UnitCommand boundCommand;
	private PlayerInputController parentPanelInputController;

	[SerializeField] private Image weaponImage;
	[SerializeField] private TextMeshProUGUI weaponText;
	[SerializeField] private GameObject switchRight;
	[SerializeField] private GameObject switchLeft;

	public string highlightColorHex;

	void Awake() {
		parentPanelInputController = GetComponentInParent<UnitCommandPanel>().inputController;
	}

	public void ActivateSwitcher(PlayerUnit thisUnit, UnitCommand thisCommand) {
		RectTransform _rt = GetComponent<RectTransform>();
		RectTransform _parent_rt = GetComponentInParent<UnitCommandPanel>().GetComponent<RectTransform>();
		_rt.position = new Vector2(_parent_rt.position.x, _rt.position.y);

		boundUnit = thisUnit;
		boundCommand = thisCommand;
		gameObject.SetActive(true);

		UpdateVisual(thisUnit);

		// bindings for keyboard
		if (thisUnit.inventory.numWeapons > 1) {
			parentPanelInputController.NextWeaponEvent += NextWeapon;
			parentPanelInputController.PrevWeaponEvent += PrevWeapon;
		}

		// if there are multiple weapons to be switched to, activate the indicator for visualization
		switchRight.SetActive(thisUnit.inventory.numWeapons > 1);
		switchLeft.SetActive(thisUnit.inventory.numWeapons > 1);
	}

	public void DeactivateSwitcher(PlayerUnit thisUnit, UnitCommand thisCommand) {
		boundUnit = null;
		boundCommand = null;
		gameObject.SetActive(false);

		// bindings for keyboard
		if (thisUnit.inventory.numWeapons > 1) {
			parentPanelInputController.NextWeaponEvent -= NextWeapon;
			parentPanelInputController.PrevWeaponEvent -= PrevWeapon;
		}
	}

	public void NextWeapon() {
		boundUnit.inventory.NextWeapon();
		UpdateVisual(boundUnit);
		//
		boundCommand.Deactivate(boundUnit);
		boundCommand.Activate(boundUnit);
	}

	public void PrevWeapon() {
		boundUnit.inventory.PrevWeapon();
		UpdateVisual(boundUnit);		
		//
		boundCommand.Deactivate(boundUnit);
		boundCommand.Activate(boundUnit);
	}

	private void UpdateVisual(PlayerUnit thisUnit) {
		string title = $"<size={weaponText.fontSize + 8}><color=#{highlightColorHex}>{thisUnit.EquippedWeapon.name}</color></size>\n";
		string dmg = $"<color=#{highlightColorHex}>Damage</color>\t[{thisUnit.EquippedWeapon.MIN_MIGHT} - {thisUnit.EquippedWeapon.MAX_MIGHT}] + {thisUnit.unitStats.STRENGTH}";
		string rng = $"<color=#{highlightColorHex}>Range</color>\t[{thisUnit.EquippedWeapon.MIN_RANGE} - {thisUnit.EquippedWeapon.MAX_RANGE}]";
		//
		weaponText.SetText(string.Join("\n", title, dmg, rng));

		weaponImage.sprite = thisUnit.EquippedWeapon.sprite;
	}
}
