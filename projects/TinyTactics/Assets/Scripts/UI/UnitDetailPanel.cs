using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitDetailPanel : MonoBehaviour
{
	public TextMeshProUGUI nameValue;
	public TextMeshProUGUI classValue;
    public Image portraitImage;

	public WeaponListing weaponListing;
	
	// attributes
	public TextMeshProUGUI hpValue;
	public TextMeshProUGUI vitValue;
	public TextMeshProUGUI strValue;
	public TextMeshProUGUI dexValue;
	public TextMeshProUGUI refValue;
	public TextMeshProUGUI defValue;
	public TextMeshProUGUI movValue;
	
	// derived
	public TextMeshProUGUI atkValue;
	public TextMeshProUGUI hitValue;
	public TextMeshProUGUI avoValue;

	// perk listing
	public GameObject perksPanel;
	[SerializeField] private PerkListing perkListingPrefab;

	public void SetUnitInfo(Unit unit) {
		if (Campaign.active != null && unit.GetType() == typeof(PlayerUnit)) {
			classValue.gameObject.SetActive(true);
			//
			var unitData = Campaign.active.UnitByID( (unit as PlayerUnit).CampaignID );
			nameValue.SetText(unitData.unitName);
			classValue.SetText(unitData.className);
		} else {
			classValue.gameObject.SetActive(false);
			//
			nameValue.SetText(unit.displayName);
		}
		
		
		//
	    portraitImage.sprite = unit.spriteRenderer.sprite;
		portraitImage.color = unit.spriteRenderer.color;
		
		//
		weaponListing.SetWeaponInfo(unit.equippedWeapon);
		
		// attributes
		hpValue.SetText($"{unit.unitStats._CURRENT_HP}/{unit.unitStats.VITALITY}");
		//
		vitValue.SetText($"{unit.unitStats.VITALITY}");
		strValue.SetText($"{unit.unitStats.STRENGTH}");
		dexValue.SetText($"{unit.unitStats.DEXTERITY}");
		refValue.SetText($"{unit.unitStats.REFLEX}");
		defValue.SetText($"{unit.unitStats.DEFENSE}");
		movValue.SetText($"{unit.unitStats.MOVE}");

		// derived
		atkValue.SetText($"{unit.unitStats._ATK}");
		hitValue.SetText($"{unit.unitStats._HIT}");
		avoValue.SetText($"{unit.unitStats._AVO}");

		// perks (was IMutatorComponent)
		foreach (PerkListing previousListing in GetComponentsInChildren<PerkListing>()) {
			Destroy(previousListing.gameObject);
		}
		foreach (Perk mc in unit.GetComponentsInChildren<Perk>()) {
			if (mc.displayName == "Weapon Advantage") continue;
			if (mc.displayName == "Weapon Effectiveness") continue;
			
			PerkListing pl = Instantiate(perkListingPrefab, perksPanel.transform);
			pl.SetPerkInfo(mc.PerkData);
		}
		

		// handle separately: buffs/debuffs?
	}
}