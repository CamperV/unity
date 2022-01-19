using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Extensions;

public class DraftPerkPanel : MonoBehaviour
{
    public TextMeshProUGUI nameValue;
    public Image perkImage;
    public TextMeshProUGUI descriptionValue;

    public Image matteImage;
    public Image backgroundImage;
    public Button draftButton;
    
    // update UI stuff
    public void SetPerkInfo(PerkData perkData) {
        nameValue.SetText(perkData.perkName);
        perkImage.sprite = perkData.sprite;
        descriptionValue.SetText(perkData.description);
        //
        // nameValue.color = Color.white;
        // perkImage.color = Color.Lerp(Color.white, perkData.belongsToArchetype.color, 0.5f);
        // descriptionValue.color = perkData.belongsToArchetype.color;
        //
        matteImage.color = perkData.belongsToArchetype.color;
        backgroundImage.color = (0.35f*perkData.belongsToArchetype.color).WithAlpha(1f);
    }
}