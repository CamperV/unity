using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Extensions;

public class DraftUnitPanel : MonoBehaviour
{
    // for coloring archetype backgrounds
    public Color AssaultColor;
    public Color DefenderColor;
    public Color SupportColor;
    public Color CunningColor;
    public Color QuickColor;

    public Color PierceColor;
    public Color SlashColor;
    public Color StrikeColor;
    public Color OtherColor;

    public TextMeshProUGUI nameValue;
    public Image portraitValue;

    public GameObject weaponDisplay;

    public GameObject archetypeDisplay;
    public TextMeshProUGUI natureDescription;

    public Button draftButton;
    
    // update UI stuff
    public void SetUnitInfo(CampaignUnitGenerator.CampaignUnitPackage unitPackage) {
        // SET NAME DATA
        nameValue.SetText($"{unitPackage.unitPrefab.displayName}");

        // SET PORTRAIT DATA
        portraitValue.sprite = unitPackage.unitPrefab.GetComponent<SpriteRenderer>().sprite;
        portraitValue.color = unitPackage.unitPrefab.GetComponent<SpriteRenderer>().color;

        // SET WEAPON DATA
        string weaponTagString = string.Join(" + ", unitPackage.unitPrefab.equippedWeapon.tags);
        weaponDisplay.GetComponentInChildren<TextMeshProUGUI>().SetText(weaponTagString);

        if (unitPackage.unitPrefab.equippedWeapon.HasTagMatch("Pierce")) {
            weaponDisplay.GetComponentInChildren<TextMeshProUGUI>().color = PierceColor;
            // weaponDisplay.GetComponentInChildren<Image>().color = 0.45f*PierceColor.WithAlpha(1f);

        } else if (unitPackage.unitPrefab.equippedWeapon.HasTagMatch("Slash")) {
            weaponDisplay.GetComponentInChildren<TextMeshProUGUI>().color = SlashColor;
            // weaponDisplay.GetComponentInChildren<Image>().color = 0.45f*SlashColor.WithAlpha(1f);

        } else if (unitPackage.unitPrefab.equippedWeapon.HasTagMatch("Strike")) {
            weaponDisplay.GetComponentInChildren<TextMeshProUGUI>().color = StrikeColor;
            // weaponDisplay.GetComponentInChildren<Image>().color = 0.45f*StrikeColor.WithAlpha(1f);

        } else {
            weaponDisplay.GetComponentInChildren<TextMeshProUGUI>().color = OtherColor;
            // weaponDisplay.GetComponentInChildren<Image>().color = 0.45f*OtherColor.WithAlpha(1f);
        }

        // weaponDisplay.GetComponentInChildren<TextMeshProUGUI>().SetText(unitPackage.unitPrefab.equippedWeapon.displayName);

        // SET ARCHETYPE DATA
        // should be in order here
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

            // SET NATURE DATA
            natureDescription.SetText(unitPackage.unitData.nature.description);
        }
    }
}