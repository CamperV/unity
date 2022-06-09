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

	public MartialWeaponListing weaponListing;
	
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
	    portraitImage.sprite = unit.spriteAnimator.MainSprite;
		portraitImage.color = unit.spriteAnimator.MainColor;
		
		//
		// weaponListing.SetWeaponInfo(unit.equippedWeapon);
		
		// attributes
		hpValue.SetText($"{unit.unitStats._CURRENT_HP}/{unit.unitStats.VITALITY}");		
		//
		vitValue.SetText($"<b>{unit.unitStats.VITALITY}</b>");
		strValue.SetText($"<b>{unit.unitStats.STRENGTH}</b>");
		dexValue.SetText($"<b>{unit.unitStats.DEXTERITY}</b>");
		refValue.SetText($"<b>{unit.unitStats.REFLEX}</b>");
		defValue.SetText($"<b>{unit.unitStats.DEFENSE}</b>");
		movValue.SetText($"<b>{unit.unitStats.MOVE}</b>");

		// derived
		atkValue.SetText($"{unit.unitStats.Calculate_ATK()}");
		hitValue.SetText($"{unit.unitStats.Calculate_HIT()}");
		avoValue.SetText($"{unit.unitStats.Calculate_AVO()}");

		string redHex = "#FF6D6D";
		string greenHex = "#6FD66E";
		// string grayHex = "#808080";
		string yellowHex = "#FFEA94";

		// handle buffs/debuffs after stats, so basically re-write them
		Dictionary<string, int> statVSMap = new Dictionary<string, int>();

		foreach (ValuedStatus vs in unit.GetComponentsInChildren<ValuedStatus>()) {
			if (!statVSMap.ContainsKey(vs.affectedStat)) statVSMap[vs.affectedStat] = 0;
			statVSMap[vs.affectedStat] += vs.modifierValue;
		}

		foreach (string affectedStat in statVSMap.Keys) {
			int modifierValue = statVSMap[affectedStat];
			if (modifierValue == 0) continue;
		
			string _color = (modifierValue > 0) ? greenHex : redHex;
			string _visual = (modifierValue > 0) ? $"(+{modifierValue})" : $"({modifierValue})";

			switch (affectedStat) {
				case "VIT":
					vitValue.SetText($"<color={yellowHex}>{_visual, -5}</color>   <b><color={_color}>{unit.unitStats.VITALITY}</color></b>");
					break;
				case "STR":
					strValue.SetText($"<color={yellowHex}>{_visual, -5}</color>   <b><color={_color}>{unit.unitStats.STRENGTH}</color></b>");				
					break;	
				case "DEX":
					dexValue.SetText($"<color={yellowHex}>{_visual, -5}</color>   <b><color={_color}>{unit.unitStats.DEXTERITY}</color></b>");
					break;
				case "REF":
					refValue.SetText($"<color={yellowHex}>{_visual, -5}</color>   <b><color={_color}>{unit.unitStats.REFLEX}</color></b>");
					break;
				case "DEF":
					defValue.SetText($"<color={yellowHex}>{_visual, -5}</color>   <b><color={_color}>{unit.unitStats.DEFENSE}</color></b>");
					break;
				case "MOV":
					movValue.SetText($"<color={yellowHex}>{_visual, -5}</color>   <b><color={_color}>{unit.unitStats.MOVE}</color></b>");
					break;
			}
		}

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
	}
}