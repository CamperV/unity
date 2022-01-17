using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DraftedUnitListing : MonoBehaviour
{
    public Color AssaultColor;
    public Color DefenderColor;
    public Color SupportColor;
    public Color CunningColor;
    public Color QuickColor;

    public TextMeshProUGUI nameValue;
    public GameObject archetypeDisplay;

    public void SetUnitInfo(CampaignUnitGenerator.CampaignUnitPackage unitPackage) {
        nameValue.SetText($"{unitPackage.unitPrefab.displayName}");

        List<Transform> archetypeChildren = new List<Transform>();
        foreach (Transform rt in archetypeDisplay.transform) {
            archetypeChildren.Add(rt);
        }

        // use this iterator to access the gameobjects above
        for (int a = 0; a < unitPackage.unitData.archetypes.Length; a++) {
            string archetype = unitPackage.unitData.archetypes[a];

            // I'm too lazy to set up a dictioanry, switch statement it is
            Color appropriateColor = Color.black;
            switch (archetype) {
                case "Assault":
                    appropriateColor = AssaultColor;
                    break;
                case "Defender":
                    appropriateColor = DefenderColor;
                    break;
                case "Support":
                    appropriateColor = SupportColor;
                    break;
                case "Cunning":
                    appropriateColor = CunningColor;
                    break;
                case "Quick":
                    appropriateColor = QuickColor;
                    break;
                case "Default":
                    Debug.Log($"ERROR: NO ARCHETYPE NAMED {archetype}");
                    break;
            }

            // by default, all but one of these children are "off"
            // this is to make having one archetype the default for the display
            var targetTransform = archetypeChildren[a];
            targetTransform.gameObject.SetActive(true);

            TextMeshProUGUI archetypeName = targetTransform.GetComponentInChildren<TextMeshProUGUI>();
            Image archetypeBackground = targetTransform.GetComponentInChildren<Image>();

            archetypeBackground.color = appropriateColor;
            archetypeName.SetText(archetype);
        }
    }
}
