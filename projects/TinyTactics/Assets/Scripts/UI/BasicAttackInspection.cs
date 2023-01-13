using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(UIBobber))]
public class BasicAttackInspection : UnitInspector
{
	public TextMeshProUGUI atkValue;
	public GameObject critContainer;
	public TextMeshProUGUI critValue;

	public override void SetUnitInfo(Unit unit) {
		// stats first
		Pair<int, int> dmgRange = unit.EquippedWeapon.DamageRange;
		if (dmgRange.First == dmgRange.Second) {
			atkValue.SetText($"{dmgRange.First}");
		} else {
			atkValue.SetText($"{dmgRange.First} - {dmgRange.Second}");
		}

		int _critValue = unit.EquippedWeapon.CRITICAL;
		if (_critValue > 0) {
			critContainer.SetActive(true);
			critValue.SetText($"{_critValue}");
		} else {
			critContainer.SetActive(false);
		}

		// then, position yourself above the unit				
		GetComponent<UIBobber>().AnchorTo(unit.transform);
		GetComponent<UIBobber>().MoveAnchorOffset(unit.transform.position, 1.0f*Vector3.up);
	}
}