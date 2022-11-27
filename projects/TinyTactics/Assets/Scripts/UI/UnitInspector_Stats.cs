using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitInspector_Stats : UnitInspector
{
	[SerializeField] private Image weaponImage;	
	[SerializeField] private TextMeshProUGUI weaponName_TMP;
	[SerializeField] private TextMeshProUGUI weaponDamage_TMP;
	[SerializeField] private TextMeshProUGUI weaponRange_TMP;
	[SerializeField] private TextMeshProUGUI weaponCrit_TMP;

	public override void SetUnitInfo(Unit unit) {
		weaponImage.sprite = unit.EquippedWeapon.sprite;
		weaponName_TMP.SetText(unit.EquippedWeapon.name);

		// stats
		weaponDamage_TMP.SetText(unit.EquippedWeapon.DisplayRawDamage(unit));
		
		Pair<int, int> range = new Pair<int, int>(unit.EquippedWeapon.MIN_RANGE, unit.EquippedWeapon.MAX_RANGE);
		string rangeExt = (range.First == range.Second) ? "" : $" - {range.Second}";
		string rng = $"{range.First}{rangeExt}";
		weaponRange_TMP.SetText(rng);

		weaponCrit_TMP.SetText($"{unit.EquippedWeapon.CRITICAL}");
	}
}