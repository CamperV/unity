using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DraftUnitPanel : MonoBehaviour
{
    public TextMeshProUGUI nameValue;
    public Image portraitValue;
    public Button draftButton;
    
    // update UI stuff
    public void SetUnitInfo(PlayerUnit unit) {
        
        nameValue.SetText($"{unit.displayName}");

        portraitValue.sprite = unit.GetComponent<SpriteRenderer>().sprite;
        portraitValue.color = unit.GetComponent<SpriteRenderer>().color;
    }
}