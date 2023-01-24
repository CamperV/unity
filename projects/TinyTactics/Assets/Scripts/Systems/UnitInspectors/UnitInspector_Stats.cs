using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.Events;

public class UnitInspector_Stats : MonoBehaviour, IUnitInspector
{
	[SerializeField] private Image weaponImage;	
	[SerializeField] private TextMeshProUGUI weaponTags_TMP;
	[SerializeField] private TextMeshProUGUI weaponName_TMP;
	[SerializeField] private TextMeshProUGUI weaponDamage_TMP;
	[SerializeField] private TextMeshProUGUI weaponRange_TMP;
	[SerializeField] private TextMeshProUGUI weaponCrit_TMP;

	// flow down to WeaponSwitcherUI, etc
	public UnityEvent<Unit> PropagateInspectUnit;

	[SerializeField] private GameObject mutationContainer;
	[SerializeField] private MutationVisual mutationVisualPrefab;

	// this is so you can detach yourself from the unit's events
	private Unit AttachedUnit => GetComponentInParent<UnitInspectorSystem>().currentUnit;
	void OnDisable() => AttachedUnit.inventory.InventoryChanged -= RefreshWeaponInfo;

	void OnEnable() {
		foreach (ContentSizeFitter csf in GetComponentsInChildren<ContentSizeFitter>()) {
			LayoutRebuilder.ForceRebuildLayoutImmediate(csf.GetComponent<RectTransform>());
		}
	}

	public void InspectUnit(Unit unit) {
		RefreshWeaponInfo(unit);
		unit.inventory.InventoryChanged += RefreshWeaponInfo;
	
		// finally, mutation container
		// quick clear, then:
		foreach (Transform t in mutationContainer.transform) {
			Destroy(t.gameObject);
		}
		foreach (Mutation mut in unit.mutationSystem.mutations) {
			MutationVisual mutationVisual = Instantiate(mutationVisualPrefab, mutationContainer.transform);
			mutationVisual.SetInfo(mut);
		}

		PropagateInspectUnit?.Invoke(unit);
	}

	// separated because WeaponSwitcher will call this
	private void RefreshWeaponInfo(Unit unit) {
		weaponImage.sprite = unit.EquippedWeapon.sprite;
		weaponName_TMP.SetText(unit.EquippedWeapon.name);

		// "tags" actually Mutations
		List<string> tags = unit.EquippedWeapon.attachedMutations.Select(mut => mut.name).ToList();
		weaponTags_TMP.SetText(string.Join(", ", tags));

		// stats
		weaponDamage_TMP.SetText(unit.EquippedWeapon.DisplayDamageRange());
		weaponRange_TMP.SetText(unit.EquippedWeapon.DisplayRange());
		weaponCrit_TMP.SetText($"{unit.EquippedWeapon.CRIT}");
	}

}