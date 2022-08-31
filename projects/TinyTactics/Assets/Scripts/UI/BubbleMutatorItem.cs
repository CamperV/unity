using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BubbleMutatorItem : MonoBehaviour
{
    public void SetText(string message) {
        GetComponentInChildren<TextMeshProUGUI>().SetText(message);
    }
}