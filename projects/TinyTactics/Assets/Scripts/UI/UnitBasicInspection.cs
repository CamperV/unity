using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitBasicInspection : MonoBehaviour
{
	public GameObject nameContainer;
	public GameObject classContainer;
	public TextMeshProUGUI nameValue;
	public TextMeshProUGUI classValue;
    public Image portraitImage;

	
	// attributes
	public TextMeshProUGUI hpValue;
	public TextMeshProUGUI defValue;
	public TextMeshProUGUI movValue;
	public TextMeshProUGUI atkValue;
	public TextMeshProUGUI critValue;

	// perk listing
	public GameObject perksPanel;
	[SerializeField] private PerkListing perkListingPrefab;

	public void SetUnitInfo(Unit unit) {
		if (Campaign.active != null && unit.GetType() == typeof(PlayerUnit)) {
			classContainer.SetActive(true);
			//
			var unitData = Campaign.active.UnitByID( (unit as PlayerUnit).CampaignID );
			nameValue.SetText(unitData.unitName);
			classValue.SetText(unitData.className);
		} else {
			classContainer.SetActive(false);
			//
			nameValue.SetText(unit.displayName);
		}
		
		//
	    portraitImage.sprite = unit.spriteAnimator.MainSprite;
		
		// attributes
		hpValue.SetText($"{unit.unitStats._CURRENT_HP}/{unit.unitStats.VITALITY}");		
		defValue.SetText($"<b>{unit.unitStats.DEFENSE}</b>");
		movValue.SetText($"<b>{unit.unitStats.MOVE}</b>");

		// derived
		int lowDmg = unit.unitStats.DEXTERITY + unit.equippedWeapon.weaponStats.MIN_MIGHT;
		int highDmg = unit.unitStats.STRENGTH + unit.equippedWeapon.weaponStats.MAX_MIGHT;
		atkValue.SetText($"{lowDmg} - {highDmg}");

		critValue.SetText($"{unit.equippedWeapon.weaponStats.CRITICAL}%");

		// perks (was IMutatorComponent)
		Perk[] perks = unit.GetComponentsInChildren<Perk>();
		if (perks.Length > 0) {
			perksPanel.SetActive(true);

			foreach (PerkListing previousListing in GetComponentsInChildren<PerkListing>()) {
				Destroy(previousListing.gameObject);
			}
			foreach (Perk mc in perks) {
				PerkListing pl = Instantiate(perkListingPrefab, perksPanel.transform);
				pl.SetPerkInfo(mc.PerkData);
			}
		} else {
			perksPanel.SetActive(false);
		}
	}
}