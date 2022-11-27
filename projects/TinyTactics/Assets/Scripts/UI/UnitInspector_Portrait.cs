using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitInspector_Portrait : UnitInspector
{
	[SerializeField] private TextMeshProUGUI name_TMP;
	[SerializeField] private Image portrait;
	[SerializeField] private TextMeshProUGUI tags_TMP;
	[SerializeField] private TextMeshProUGUI health_TMP;
	[SerializeField] private TextMeshProUGUI damageReduction_TMP;
	[SerializeField] private TextMeshProUGUI break_TMP;

	[SerializeField] private SegmentedHealthBarUI healthBar;
	[SerializeField] private BreakBarUI breakBar;
	[SerializeField] private StatusBarUI statusBar;


	public override void SetUnitInfo(Unit unit) {
		name_TMP.SetText(unit.displayName);
		portrait.sprite = unit.portraitSprite;
		tags_TMP.SetText( string.Join(", ", unit.tags) );

		healthBar.AttachTo(unit);
		breakBar.AttachTo(unit);
		statusBar.AttachTo(unit);

		health_TMP.SetText($"{unit.unitStats._CURRENT_HP}/{unit.unitStats.VITALITY}");
		damageReduction_TMP.SetText($"{unit.unitStats.DEFENSE}");
		break_TMP.SetText($"{unit.unitStats._CURRENT_BREAK}/{unit.unitStats.BRAWN}");
	}
}