using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public interface IEquipable
{
	void Equip(Unit onUnit);
	void Unequip(Unit fromUnit);
}