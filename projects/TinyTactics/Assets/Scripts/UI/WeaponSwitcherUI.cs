using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Extensions;

public class WeaponSwitcherUI : MonoBehaviour
{
	[SerializeField] private PlayerInputController playerInputController;
	[SerializeField] private UnitInspector unitInspector;

	[SerializeField] private GameObject switchRight;
	[SerializeField] private GameObject switchLeft;

	private Unit boundUnit;

	public void AttachTo(Unit thisUnit) {
		boundUnit = thisUnit;
		UpdateVisual(boundUnit);

		// bindings for keyboard
		if (thisUnit.inventory.numWeapons > 1) {
			playerInputController.NextWeaponEvent += NextWeapon;
			playerInputController.PrevWeaponEvent += PrevWeapon;
		}

		// if there are multiple weapons to be switched to, activate the indicator for visualization
		switchRight.SetActive(thisUnit.inventory.numWeapons > 1);
		switchLeft.SetActive(thisUnit.inventory.numWeapons > 1);
	}

	public void Detach() {
		if (boundUnit.inventory.numWeapons > 1) {
			playerInputController.NextWeaponEvent -= NextWeapon;
			playerInputController.PrevWeaponEvent -= PrevWeapon;
		}
		boundUnit = null;
	}

	public void NextWeapon() {
		boundUnit.inventory.NextWeapon();
		UpdateVisual(boundUnit);
		//
		// boundCommand.Deactivate(boundUnit);
		// boundCommand.Activate(boundUnit);
	}

	public void PrevWeapon() {
		boundUnit.inventory.PrevWeapon();
		UpdateVisual(boundUnit);		
		//
		// boundCommand.Deactivate(boundUnit);
		// boundCommand.Activate(boundUnit);
	}

	private void UpdateVisual(Unit boundUnit) {
		unitInspector.SetUnitInfo(boundUnit);
	}
}
