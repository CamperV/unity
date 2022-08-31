using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ListMutatorDisplay : UIMutatorDisplay
{
    [SerializeField] private GameObject container;
    [SerializeField] private TextMeshProUGUI listText;

    public override void DisplayMutators(List<string> mutators) {
        Clear();

        if (mutators.Count > 0) {
            container.SetActive(true);
            listText.SetText( string.Join("\n", mutators) );
        }
    }

    private void Clear() {
        container.SetActive(false);
        listText.SetText("");
    }
}