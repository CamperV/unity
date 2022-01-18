using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveCampaignUnitListing : MonoBehaviour
{   
    [SerializeField] private GameObject unitListingDisplay;
    [SerializeField] private DraftedUnitListing draftedUnitListingPrefab;

    void OnEnable() {
        foreach (CampaignUnitGenerator.CampaignUnitData unit in Campaign.active.Units) {
            DraftedUnitListing listing = Instantiate(draftedUnitListingPrefab, unitListingDisplay.transform);
            listing.SetUnitInfo(unit);
        }
    }
}
