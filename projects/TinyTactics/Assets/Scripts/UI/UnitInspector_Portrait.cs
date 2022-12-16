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
	[SerializeField] private PoiseBar_UI poiseBar;
	[SerializeField] private StatusBarUI statusBar;


	public override void SetUnitInfo(Unit unit) {
		name_TMP.SetText(unit.displayName);
		portrait.sprite = unit.portraitSprite;
		tags_TMP.SetText( string.Join(", ", unit.tags) );

		healthBar.AttachTo(unit);
		poiseBar.AttachTo(unit);
		statusBar.AttachTo(unit);

		health_TMP.SetText($"{unit.statSystem.CURRENT_HP}/{unit.statSystem.MAX_HP}");
		damageReduction_TMP.SetText($"{unit.statSystem.DAMAGE_REDUCTION}");
		break_TMP.SetText($"{unit.statSystem.CURRENT_POISE}/{unit.statSystem.MAX_POISE}");
	}
}