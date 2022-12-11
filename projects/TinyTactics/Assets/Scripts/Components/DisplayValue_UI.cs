using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Linq;
using Extensions;
using TMPro;

public class DisplayValue_UI : MonoBehaviour
{	
    public enum RegistrationOptions {
        UpdateHPEvent,
        UpdateBreakEvent,
        UpdateDefenseEvent
    };
    [SerializeField] private RegistrationOptions registerTo;

    [SerializeField] private TextMeshProUGUI value;
    [SerializeField] private int maxValue;

    public void AttachTo(Unit thisUnit) {
        switch (registerTo) {
            case RegistrationOptions.UpdateHPEvent:
                UpdateValue(thisUnit.unitStats.VITALITY, thisUnit.unitStats.VITALITY);
                thisUnit.unitStats.UpdateHPEvent += UpdateValue;
                break;

            case RegistrationOptions.UpdateBreakEvent:
                UpdateValue(thisUnit.unitStats.BRAWN, thisUnit.unitStats.BRAWN);
                thisUnit.unitStats.UpdateBreakEvent += UpdateValue;
                break;

            case RegistrationOptions.UpdateDefenseEvent:
                UpdateValue(thisUnit.unitStats.DEFENSE, thisUnit.unitStats.DEFENSE);
                thisUnit.unitStats.UpdateDefenseEvent += d => UpdateValue(d, 0);
                break;

            default:
                break;
        }
    }

    private void UpdateValue(int val, int max) {
        string final = (val <= maxValue) ? $"{val}" : "??";
        value.SetText(final);
    }
}
