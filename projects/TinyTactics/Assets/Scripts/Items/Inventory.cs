using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Extensions;

public class Inventory : MonoBehaviour
{
	// public delegate InventoryEvent(Unit thisUnit);
	// public event InventoryEvent InventoryChanged;

	private Unit boundUnit;
	
	// set in inspector
	[SerializeField] private List<Weapon> weapons;
	public Weapon FirstWeapon => weapons[0];
	public IEnumerable<Weapon> Weapons => weapons.AsEnumerable();

	public int numWeapons => weapons.Count;

	void Awake() {
		boundUnit = GetComponent<Unit>();
	}

	public void Initialize() {
		FirstWeapon.Equip(boundUnit);
	}

	public void NextWeapon() {
		FirstWeapon.Unequip(boundUnit);
		weapons = weapons.Roll(1);

		FirstWeapon.Equip(boundUnit);
        boundUnit.personalAudioFX.PlayWeaponEquipFX();
	}

	public void PrevWeapon() {
		FirstWeapon.Unequip(boundUnit);
		weapons = weapons.Roll(-1);

		FirstWeapon.Equip(boundUnit);
		boundUnit.personalAudioFX.PlayWeaponEquipFX();
	}
}



