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
	// public 
	[SerializeField] private PlayerInputController playerInputController;
	[SerializeField] private UnitInspector_Stats unitInspector;

	[SerializeField] private GameObject switchRight;
	[SerializeField] private GameObject switchLeft;

	private Unit boundUnit;

	void OnDisable() => Detach();

	public void AttachTo(Unit thisUnit) {
		boundUnit = thisUnit;

		// bindings for keyboard
		if (thisUnit.inventory.NumWeapons > 1) {
			playerInputController.NextWeaponEvent += NextWeapon;
			playerInputController.PrevWeaponEvent += PrevWeapon;
		}

		// if there are multiple weapons to be switched to, activate the indicator for visualization
		switchRight.SetActive(thisUnit.inventory.NumWeapons > 1);
		switchLeft.SetActive(thisUnit.inventory.NumWeapons > 1);
	}

	public void Detach() {
		if (boundUnit.inventory.NumWeapons > 1) {
			playerInputController.NextWeaponEvent -= NextWeapon;
			playerInputController.PrevWeaponEvent -= PrevWeapon;
		}
		boundUnit = null;
	}

	public void NextWeapon() => boundUnit.inventory.NextWeapon();
	public void PrevWeapon() => boundUnit.inventory.PrevWeapon();
}
