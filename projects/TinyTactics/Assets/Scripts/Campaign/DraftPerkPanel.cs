using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Extensions;

public class DraftPerkPanel : MonoBehaviour
{
    public TextMeshProUGUI nameValue;
    public TextMeshProUGUI descriptionValue;

    public Image backgroundImage;
    public Button draftButton;
    
    // update UI stuff
    public void SetPerkInfo(PerkData perkData) {
        nameValue.SetText(perkData.perkName);
        descriptionValue.SetText(perkData.description);
        //
        nameValue.color = perkData.belongsToArchetype.color;
        backgroundImage.color = 0.75f*perkData.belongsToArchetype.color;
    }
}