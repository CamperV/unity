using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Extensions;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UnitLevelUpSystem : MonoBehaviour
{
    [SerializeField] private EventManager eventManager;
    [SerializeField] private EngagementSystem engagementSystem;

    [SerializeField] private GameObject levelUpPanel;
    [SerializeField] private MutationPanel mutationPanelPrefab;

    public void CatchLevelUp(Unit unit) {
        Debug.Log($"Saw {unit} level up");
        StartCoroutine( InteractiveMutationOffer() );
    }

    private void EnablePanel() {
        eventManager.DisablePlayerInput();
        eventManager.EnableMenuInput();
        levelUpPanel.SetActive(true);
    }

    private void DisablePanel() {
        eventManager.DisableMenuInput();
        eventManager.EnablePlayerInput();
        levelUpPanel.SetActive(false);       
    }

    private IEnumerator InteractiveMutationOffer() {
        yield return new WaitUntil(() => engagementSystem.IsResolved == true);
        EnablePanel();
        yield return new WaitForSeconds(5.0f);
        DisablePanel();
    }

    // private IEnumerator InteractivePerkDraft(CampaignUnitGenerator.CampaignUnitData unitData, UnitLevelUpPanel linkedPanel) {
    //     if (!unitPerkOfferings.ContainsKey(unitData.ID)) {
    //         List<PerkData> potentialPerkPool = new List<PerkData>();

    //         foreach (ArchetypeData ad in unitData.archetypes) {
    //             foreach (PerkData draftablePerk in ad.GetPerkPool()) {
    //                 potentialPerkPool.Add(draftablePerk);
    //             }
    //         }
    //         //
    //         unitPerkOfferings[unitData.ID] = potentialPerkPool.RandomSelectionsUpTo<PerkData>(perksOnOffer).ToList();
    //     }
    //     List<PerkData> perkPool = unitPerkOfferings[unitData.ID];

    //     // offer up all perks and wait until one is pressed
    //     bool buttonClickedFlag = false;

    //     // instantiate all possible panels for this draft
    //     // show all panels
    //     foreach (PerkData draftablePerk in perkPool) {
    //         DraftPerkPanel perkPanel = Instantiate(draftPerkPanelPrefab, linkedPanel.perkDraftTable.transform);
    //         perkPanel.SetPerkInfo(draftablePerk);

    //         perkPanel.draftButton.onClick.AddListener(() => {
    //             AddPerkToUnit(unitData, draftablePerk);
    //             //
    //             unitTabs.Remove(currentSelectedTab);
    //             Destroy(currentSelectedTab.gameObject);
    //             //
    //             buttonClickedFlag = true;

    //             if (unitTabs.Count > 0) {
    //                 SelectTab(unitTabs[0]);
    //             } else {
    //                 Destroy(gameObject);
    //             }
    //         });
    //     }

    //     // now wait until a unit is actually selected
    //     yield return new WaitUntil(() => buttonClickedFlag == true);
    // }

}