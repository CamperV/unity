using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Extensions;

public class DraftUnitPanel : MonoBehaviour
{
    public TextMeshProUGUI nameValue;
    public TextMeshProUGUI classValue;
    public Image portraitValue;

    public GameObject archetypeDisplay;
    public TextMeshProUGUI natureDescription;

    public TextMeshProUGUI signaturePerkDescription;

    public Button draftButton;
    
    // update UI stuff
    public void SetUnitInfo(CampaignUnitGenerator.CampaignUnitPackage unitPackage) {

        // SET NAME DATA
        nameValue.SetText($"{unitPackage.unitData.unitName}");
        classValue.SetText($"{unitPackage.unitData.className}");


        // SET PORTRAIT DATA
        portraitValue.sprite = unitPackage.unitPrefab.GetComponent<SpriteRenderer>().sprite;
        portraitValue.color = unitPackage.unitPrefab.GetComponent<SpriteRenderer>().color;


        // SET ARCHETYPE DATA
        // should be in order here
        List<Transform> archetypeChildren = new List<Transform>();
        foreach (Transform rt in archetypeDisplay.transform) {
            archetypeChildren.Add(rt);
        }

        // use this iterator to access the gameobjects above
        for (int a = 0; a < unitPackage.unitData.archetypes.Length; a++) {
            ArchetypeData archetypeData = unitPackage.unitData.archetypes[a];

            // by default, all but one of these children are "off"
            // this is to make having one archetype the default for the display
            var targetTransform = archetypeChildren[a];
            targetTransform.gameObject.SetActive(true);

            TextMeshProUGUI archetypeName = targetTransform.GetComponentInChildren<TextMeshProUGUI>();
            Image archetypeBackground = targetTransform.GetComponentInChildren<Image>();

            archetypeBackground.color = archetypeData.color;
            archetypeName.SetText(archetypeData.archetypeName);
        }
    

        // SET WEAPON DATA
        // weaponListing.SetWeaponInfo(unitPackage.unitPrefab.EquippedWeapon);
    
        // SET NATURE DATA
        natureDescription.SetText(unitPackage.unitData.nature.description);


        // SET SIGNATURE PERK DESCRIPTION
        // Debug.Log($"Searching for component {unitPackage.unitData.signaturePerkTypeName}");
        IToolTip describablePerk = unitPackage.unitPrefab.GetComponent(unitPackage.unitData.signaturePerkTypeName) as IToolTip;
        signaturePerkDescription.SetText($"<b><color=#FFDD70>{describablePerk.tooltipName}</color></b>: {describablePerk.tooltip}");
    }
}