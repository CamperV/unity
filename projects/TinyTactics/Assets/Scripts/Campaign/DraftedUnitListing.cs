using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DraftedUnitListing : MonoBehaviour
{
    [HideInInspector] public CampaignUnitGenerator.CampaignUnitData listedUnitData;
    public TextMeshProUGUI nameValue;
    public TextMeshProUGUI classValue;
    public GameObject archetypeDisplay;

    public void SetUnitInfo(CampaignUnitGenerator.CampaignUnitData unitData) {
        listedUnitData = unitData;

        // NAME + CLASS DATA
        nameValue.SetText($"{unitData.unitName}");
        classValue.SetText($"{unitData.className}");

        // ARCHETYPE DATA
        List<Transform> archetypeChildren = new List<Transform>();
        foreach (Transform rt in archetypeDisplay.transform) {
            archetypeChildren.Add(rt);
        }

        // use this iterator to access the gameobjects above
        for (int a = 0; a < unitData.archetypes.Length; a++) {
            ArchetypeData archetypeData = unitData.archetypes[a];

            // by default, all but one of these children are "off"
            // this is to make having one archetype the default for the display
            var targetTransform = archetypeChildren[a];
            targetTransform.gameObject.SetActive(true);

            TextMeshProUGUI archetypeName = targetTransform.GetComponentInChildren<TextMeshProUGUI>();
            Image archetypeBackground = targetTransform.GetComponentInChildren<Image>();

            archetypeBackground.color = archetypeData.color;
            archetypeName.SetText(archetypeData.archetypeName);
        }
    }
}
