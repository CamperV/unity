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
    [SerializeField] private GameObject selectionPanel;
    [SerializeField] private MutationPanel mutationPanelPrefab;

    public void CatchLevelUp(Unit unit) {
        // need some blocking code here to receive mulitple serial level ups
        StartCoroutine( InteractiveMutationOffer(unit) );
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

    private void ClearPanel() {
        foreach (Transform child in selectionPanel.transform) {
            Destroy(child.gameObject);
        }
    }

    private IEnumerator InteractiveMutationOffer(Unit unit) {
        yield return new WaitUntil(() => engagementSystem.IsResolved == true);

        EnablePanel();
        ClearPanel();
        List<Mutation> offering = GetOffering(unit, 3);

        // offer up all mutations and wait until one is pressed
        bool buttonClickedFlag = false;

        // instantiate all possible panels for this draft
        foreach (Mutation draftableMutation in offering) {
            MutationPanel mutationPanel = Instantiate(mutationPanelPrefab, selectionPanel.transform);
            mutationPanel.SetInfo(draftableMutation);
            // perkPanel.SetPerkInfo(draftablePerk);

            // don't care about anon listeners because this will be destroyed
            mutationPanel.selectionButton.onClick.AddListener(() => DraftMutation(unit, draftableMutation) );
            mutationPanel.selectionButton.onClick.AddListener(() => buttonClickedFlag = true);
        }

        // now wait until a mutation is actually selected
        yield return new WaitUntil(() => buttonClickedFlag == true);       
        DisablePanel();
    }

    private List<Mutation> GetOffering(Unit unit, int numOnOffer) {
        List<Mutation> offerings = new List<Mutation>();

        foreach (MutationArchetype mutArch in unit.mutArchetypes) {
            foreach (Mutation mut in mutArch.GetPool()) {
                offerings.Add(mut);
            }
        }

        // prune em down
        offerings.RandomSelectionsUpTo<Mutation>(numOnOffer);
        return offerings;
    }

    private void DraftMutation(Unit unit, Mutation mutation) {
        unit.mutationSystem.AddMutation(mutation);
    }
}