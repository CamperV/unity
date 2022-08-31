using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BubbleMutatorDisplay : UIMutatorDisplay
{
    [SerializeField] private GameObject container;
    [SerializeField] private BubbleMutatorItem bubblePrefab;

    public override void DisplayMutators(List<string> mutators) {
        Clear();

        if (mutators.Count > 0) {
            container.SetActive(true);
            
            foreach (string mutator in mutators) {
                BubbleMutatorItem bubble = Instantiate(bubblePrefab, container.transform);
                bubble.SetText(mutator);
            }
        }
    }

    private void Clear() {
        foreach (Transform child in container.transform) {
            Destroy(child.gameObject);
        }
        container.SetActive(false);
    }
}