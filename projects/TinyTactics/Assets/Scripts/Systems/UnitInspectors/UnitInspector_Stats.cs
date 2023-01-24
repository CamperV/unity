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

	public void InspectUnit(Unit unit) {
		RefreshWeaponInfo(unit);
		RebuildMutations(unit);

		// register these because mutations can change based on weapon
		unit.inventory.InventoryChanged += RefreshWeaponInfo;
		unit.inventory.InventoryChanged += RebuildMutations;
	
		PropagateInspectUnit?.Invoke(unit);
	}

	public void DetachListeners(Unit droppedUnit) {
		if (droppedUnit != null) {
			droppedUnit.inventory.InventoryChanged -= RefreshWeaponInfo;
			droppedUnit.inventory.InventoryChanged -= RebuildMutations;
		}
	}

	// separated because WeaponSwitcher will call this
	private void RefreshWeaponInfo(Unit unit) {
		weaponImage.sprite = unit.EquippedWeapon.sprite;
		weaponName_TMP.SetText(unit.EquippedWeapon.name);

		// "tags" actually Mutations
		List<string> tags = unit.EquippedWeapon.attachedMutations.Select(mut => mut.mutatorDisplayData.name).ToList();
		tags = tags.Concat(unit.EquippedWeapon.attachedStatuses.Select(st => st.mutatorDisplayData.name)).ToList();
		weaponTags_TMP.SetText(string.Join(", ", tags));

		// stats
		weaponDamage_TMP.SetText(unit.EquippedWeapon.DisplayDamageRange());
		weaponRange_TMP.SetText(unit.EquippedWeapon.DisplayRange());
		weaponCrit_TMP.SetText($"{unit.EquippedWeapon.CRIT}");
	}

	private void RebuildMutations(Unit unit) {
		foreach (Transform t in mutationContainer.transform) {
			Destroy(t.gameObject);
		}
		foreach (Mutation mut in unit.mutationSystem.mutations) {
			if (mut.hidden) continue;

			MutationVisual mutationVisual = Instantiate(mutationVisualPrefab, mutationContainer.transform);
			mutationVisual.SetInfo(mut);
		}

		// and rebuild layout
		foreach (ContentSizeFitter csf in GetComponentsInChildren<ContentSizeFitter>()) {
			LayoutRebuilder.ForceRebuildLayoutImmediate(csf.GetComponent<RectTransform>());
		}
	}
}