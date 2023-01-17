using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UnitInspector_Stats : UnitInspector
{
	[SerializeField] private Image weaponImage;	
	[SerializeField] private TextMeshProUGUI weaponTags_TMP;
	[SerializeField] private TextMeshProUGUI weaponName_TMP;
	[SerializeField] private TextMeshProUGUI weaponDamage_TMP;
	[SerializeField] private TextMeshProUGUI weaponRange_TMP;
	[SerializeField] private TextMeshProUGUI weaponCrit_TMP;

	[SerializeField] private WeaponSwitcherUI weaponSwitcher;

	[SerializeField] private GameObject mutationContainer;
	[SerializeField] private MutationVisual mutationVisualPrefab;

	public override void SetUnitInfo(Unit unit) {
		RefreshUnitInfo(unit);
		weaponSwitcher.AttachTo(unit);
	}

	// separated because WeaponSwitcher will call this
	public void RefreshUnitInfo(Unit unit) {
		weaponImage.sprite = unit.EquippedWeapon.sprite;
		weaponName_TMP.SetText(unit.EquippedWeapon.name);

		// "tags" actually Mutations
		List<string> tags = unit.EquippedWeapon.attachedMutations.Select(mut => mut.name).ToList();
		weaponTags_TMP.SetText(string.Join(", ", tags));

		// stats
		weaponDamage_TMP.SetText(unit.EquippedWeapon.DisplayDamageRange());
		
		Pair<int, int> range = new Pair<int, int>(unit.EquippedWeapon.MIN_RANGE, unit.EquippedWeapon.MAX_RANGE);
		string rangeExt = (range.First == range.Second) ? "" : $" - {range.Second}";
		string rng = $"{range.First}{rangeExt}";
		weaponRange_TMP.SetText(rng);

		weaponCrit_TMP.SetText($"{unit.EquippedWeapon.CRIT}");

		// finally, mutation container
		// quick clear, then:
		foreach (Transform t in mutationContainer.transform) {
			Destroy(t.gameObject);
		}
		foreach (Mutation mut in unit.mutationSystem.mutations) {
			MutationVisual mutationVisual = Instantiate(mutationVisualPrefab, mutationContainer.transform);
			mutationVisual.SetInfo(mut);
		}
	}
}