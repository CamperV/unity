using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(UIBobber))]
public class BasicAttackInspection : MonoBehaviour
{
	public TextMeshProUGUI atkValue;
	public GameObject critContainer;
	public TextMeshProUGUI critValue;

	public void SetUnitInfo(Unit unit) {
		// stats first
		Pair<int, int> dmgRange = unit.unitStats.CalculateDamageRange();
		if (dmgRange.First == dmgRange.Second) {
			atkValue.SetText($"{dmgRange.First}");
		} else {
			atkValue.SetText($"{dmgRange.First} - {dmgRange.Second}");
		}

		int _critValue = unit.unitStats.CalculateCritical();
		if (_critValue > 0) {
			critContainer.SetActive(true);
			critValue.SetText($"{_critValue}");
		} else {
			critContainer.SetActive(false);
		}

		// then, position yourself above the unit
		// transform.position = unit.transform.position + Vector3.up;
	}
}