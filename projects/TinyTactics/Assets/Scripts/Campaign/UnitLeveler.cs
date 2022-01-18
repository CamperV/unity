using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Extensions;

public class UnitLeveler : MonoBehaviour
{
    public int perksOnOffer = 3;

    [SerializeField] private GameObject tabBar;
    [SerializeField] private GameObject panelContainer;

    [SerializeField] private UnitTab unitTabPrefab;
    [SerializeField] private List<UnitTab> unitTabs;

    private UnitTab currentSelectedTab;
    private Coroutine selectedTabCrt;

    [SerializeField] private DraftPerkPanel draftPerkPanelPrefab;

    private Dictionary<Guid, List<PerkData>> unitPerkOfferings;
    private bool cancelSignal = false;

    void Awake() {
        unitPerkOfferings = new Dictionary<Guid, List<PerkData>>();
    }

    void OnEnable() {
        if (Campaign.active == null) return;
        //

        foreach (CampaignUnitGenerator.CampaignUnitData unit in Campaign.active.Units) {
            UnitTab unitTab = Instantiate(unitTabPrefab, tabBar.transform);
            unitTab.CreateLinkedPanel(panelContainer.transform);
            unitTab.SetUnitInfo(unit);

            unitTabs.Add(unitTab);
        }

        SelectTab(unitTabs[0]);
    }

    public void SelectTab(UnitTab target) {
        if (target != currentSelectedTab) {
            if (selectedTabCrt != null) {
                StopCoroutine(selectedTabCrt);
            }

            currentSelectedTab = target;
            ClearAllPanels();
            target.Select();

            // start the coroutine for this specific unit
            selectedTabCrt = StartCoroutine( InteractivePerkDraft(target.tabUnitData, target.linkedPanel) );
        }
    }

    public void ClearAllPanels() {
        // need to destroy panels first, because setting them as inactive causes them not to be found
        foreach (var panel in GetComponentsInChildren<DraftPerkPanel>()) {
            Destroy(panel.gameObject);
        }

        foreach (UnitTab ut in unitTabs) {
            ut.Deselect();
        }
    }

    private IEnumerator InteractivePerkDraft(CampaignUnitGenerator.CampaignUnitData unitData, UnitLevelUpPanel linkedPanel) {
        if (!unitPerkOfferings.ContainsKey(unitData.ID)) {
            List<PerkData> potentialPerkPool = new List<PerkData>();

            foreach (ArchetypeData ad in unitData.archetypes) {
                foreach (PerkData draftablePerk in ad.perkPool) {
                    potentialPerkPool.Add(draftablePerk);
                }
            }
            //
            unitPerkOfferings[unitData.ID] = potentialPerkPool.RandomSelections<PerkData>(perksOnOffer).ToList();
        }
        List<PerkData> perkPool = unitPerkOfferings[unitData.ID];

        // offer up all perks and wait until one is pressed
        bool buttonClickedFlag = false;

        // instantiate all possible panels for this draft
        // show all panels
        foreach (PerkData draftablePerk in perkPool) {
            DraftPerkPanel perkPanel = Instantiate(draftPerkPanelPrefab, linkedPanel.perkDraftTable.transform);
            perkPanel.SetPerkInfo(draftablePerk);

            perkPanel.draftButton.onClick.AddListener(() => {
                AddPerkToUnit(unitData, draftablePerk);
                //
                unitTabs.Remove(currentSelectedTab);
                Destroy(currentSelectedTab.gameObject);
                //
                buttonClickedFlag = true;

                if (unitTabs.Count > 0) SelectTab(unitTabs[0]);
                else Destroy(gameObject);
            });
        }

        // now wait until a unit is actually selected
        yield return new WaitUntil(() => buttonClickedFlag == true);

        foreach (var panel in GetComponentsInChildren<DraftPerkPanel>()) {
            Destroy(panel.gameObject);
        }
    }

    private void AddPerkToUnit(CampaignUnitGenerator.CampaignUnitData unitData, PerkData draftablePerk) {
        CampaignUnitGenerator.CampaignUnitData newUnitData = new CampaignUnitGenerator.CampaignUnitData(unitData);
        newUnitData.perks = newUnitData.perks.Append(draftablePerk).ToArray();
        //
        Campaign.active.SerializeUnit(newUnitData);
    }
}
