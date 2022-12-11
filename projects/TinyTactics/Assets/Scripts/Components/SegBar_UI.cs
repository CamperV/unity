using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Linq;
using Extensions;
using TMPro;

public class SegBar_UI : MonoBehaviour
{	
    public enum RegistrationOptions {
        UpdateHPEvent,
        UpdateBreakEvent,
        UpdateDefenseEvent
    };
    [SerializeField] private RegistrationOptions registerTo;

    [HideInInspector] public int currVal;
    public int truncateAfter = 4;

    [SerializeField] private Transform barLevel;
    [SerializeField] private GameObject segmentPrefab;


    public void AttachTo(Unit thisUnit) {
        switch (registerTo) {
            case RegistrationOptions.UpdateHPEvent:
                UpdateBar(thisUnit.unitStats.VITALITY, thisUnit.unitStats.VITALITY);
                thisUnit.unitStats.UpdateHPEvent += UpdateBar;
                break;

            case RegistrationOptions.UpdateBreakEvent:
                UpdateBar(thisUnit.unitStats.BRAWN, thisUnit.unitStats.BRAWN);
                thisUnit.unitStats.UpdateBreakEvent += UpdateBar;
                break;

            case RegistrationOptions.UpdateDefenseEvent:
                UpdateBar(thisUnit.unitStats.DEFENSE, thisUnit.unitStats.DEFENSE);
                thisUnit.unitStats.UpdateDefenseEvent += d => UpdateBar(d, 0);
                break;

            default:
                break;
        }
    }

    private void UpdateBar(int val, int _) {
        currVal = val;

        foreach (Transform bar in barLevel.transform) {
            if (bar.gameObject.tag == "DoNotDestroy") continue;
            Destroy(bar.gameObject);
        }
        for (int b = 0; b < currVal; b++) {
            if (b < truncateAfter) {
                var segment = Instantiate(segmentPrefab, barLevel.transform);
                segment.transform.SetSiblingIndex(0);
            }
        }
    }
}
