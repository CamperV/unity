using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TabViewController : MonoBehaviour
{
    [SerializeField] private TabViewElement[] tabViewElements;

    void Start() => ActivateTab(tabViewElements[0]);

    public void ActivateTab(TabViewElement toActivate) {
        foreach (TabViewElement tve in tabViewElements) {
            tve.Dim();
            tve.DeactivateLinkedPanel();
        }

        toActivate.Undim();
        toActivate.ActivateLinkedPanel();
    }
}
