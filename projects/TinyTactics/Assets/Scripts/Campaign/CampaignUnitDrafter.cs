using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Extensions;

public class CampaignUnitDrafter : MonoBehaviour
{
    [SerializeField] Campaign draftIntoCampaign;

    [SerializeField] private int maxUnits = 4;
    [SerializeField] private int unitsOnOffer = 3;
    //
    [SerializeField] private GameObject draftedUnitsDisplay;
    [SerializeField] private DraftedUnitListing draftedUnitListingPrefab;

    [SerializeField] private TextMeshProUGUI draftTableLabel;
    [SerializeField] private GameObject draftTable;
    [SerializeField] private PlayerUnit[] draftPrefabPool;
    [SerializeField] private DraftUnitPanel draftUnitPanelPrefab;

    [SerializeField] private GameObject[] chainedGameObjects;

    private List<CampaignUnitGenerator.CampaignUnitPackage> draftedUnits;

    void Awake() {
        draftedUnits = new List<CampaignUnitGenerator.CampaignUnitPackage>();

        // populate the drafting pool
        draftPrefabPool = Resources.LoadAll<PlayerUnit>("Units/PlayerUnits");
    }

    public void BeginDraft() => StartCoroutine(_BeginDraft());

    private IEnumerator _BeginDraft() {
        draftIntoCampaign = Campaign.active;

        // will exit after drafting max units
        yield return InteractiveDraft();

        Debug.Assert(draftedUnits.Count == maxUnits);
        foreach (var unitPackage in draftedUnits) {
            draftIntoCampaign.EnlistUnit(unitPackage.unitData);
        }

        //
        gameObject.SetActive(false);

        // set up future chained events
        foreach (GameObject go in chainedGameObjects) {
            go.SetActive(true);
        }
    }

    private IEnumerator InteractiveDraft() {
        bool buttonClickedFlag = false;

        // draft this many times
        for (int u = 0; u < maxUnits; u++) {
            draftTableLabel.SetText($"Choose One ({u+1} of {maxUnits})");
            buttonClickedFlag = false;

            // instantiate all possible panels for this draft
            List<DraftUnitPanel> panelsToDestroy = new List<DraftUnitPanel>();

            foreach (PlayerUnit unitPrefab in draftPrefabPool.RandomSelections<PlayerUnit>(unitsOnOffer)) {
                ////////////////////////////////////////////
                // CREATE UNIT DATA HERE FOR THE CAMPAIGN //
                ////////////////////////////////////////////
                var unitPackage = new CampaignUnitGenerator.CampaignUnitPackage(unitPrefab);

                DraftUnitPanel unitPanel = Instantiate(draftUnitPanelPrefab, draftTable.transform);
                unitPanel.SetUnitInfo(unitPackage);
                //
                panelsToDestroy.Add(unitPanel);

                unitPanel.draftButton.onClick.AddListener(() => {
                    DraftUnit(unitPackage);
                    buttonClickedFlag = true;
                });
            }

            // now wait until a unit is actually seleced
            yield return new WaitUntil(() => buttonClickedFlag == true);

            // now kill all the panels we created
            foreach (DraftUnitPanel unitPanel in panelsToDestroy) {
                Destroy(unitPanel.gameObject);
            }
        }
    }

    private void DraftUnit(CampaignUnitGenerator.CampaignUnitPackage unitPackage) {
        DraftedUnitListing listing = Instantiate(draftedUnitListingPrefab, draftedUnitsDisplay.transform);
        listing.SetUnitInfo(unitPackage);

        draftedUnits.Add(unitPackage);
    }
}