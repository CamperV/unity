using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Extensions;

public class UnitDrafter : MonoBehaviour
{
    [SerializeField] Campaign draftIntoCampaign;

    [SerializeField] private int maxUnits = 4;
    [SerializeField] private int unitsOnOffer = 3;
    //
    [SerializeField] private GameObject draftedUnitsDisplay;
    [SerializeField] private DraftedUnitListing draftedUnitListingPrefab;

    [SerializeField] private GameObject draftTable;
    [SerializeField] private List<PlayerUnit> draftPool;    // set in inspector
    [SerializeField] private DraftUnitPanel draftUnitPanelPrefab;

    [SerializeField] private GameObject[] chainedGameObjects;

    private List<PlayerUnit> draftedUnits;

    void Awake() {
        draftedUnits = new List<PlayerUnit>();
    }

    public void BeginDraft() => StartCoroutine(_BeginDraft());

    private IEnumerator _BeginDraft() {
        draftIntoCampaign = Campaign.active;

        // will exit after drafting max units
        yield return InteractiveDraft();

        Debug.Assert(draftedUnits.Count == maxUnits);
        draftedUnits.ForEach(u => draftIntoCampaign.EnlistUnit(u));

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
            buttonClickedFlag = false;

            // instantiate all possible panels for this draft
            List<DraftUnitPanel> panelsToDestroy = new List<DraftUnitPanel>();

            foreach (PlayerUnit unit in draftPool.RandomSelections<PlayerUnit>(unitsOnOffer)) {
                DraftUnitPanel unitPanel = Instantiate(draftUnitPanelPrefab, draftTable.transform);
                unitPanel.SetUnitInfo(unit);
                //
                panelsToDestroy.Add(unitPanel);

                unitPanel.draftButton.onClick.AddListener(() => {
                    DraftUnit(unit);
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

    private void DraftUnit(PlayerUnit unit) {
        DraftedUnitListing listing = Instantiate(draftedUnitListingPrefab, draftedUnitsDisplay.transform);
        listing.nameValue.SetText($"{unit.displayName}");

        draftedUnits.Add(unit);
    }
}