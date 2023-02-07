using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Extensions;

public class MutationPanel : MonoBehaviour
{
    public TextMeshProUGUI nameValue;
    public Image mutationImage;
    public Image mutationImageMatte;
    public TextMeshProUGUI descriptionValue;

    public Image matteImage;
    public Image backgroundImage;

    public Button selectionButton;
    
    // update UI stuff
    public void SetInfo(Mutation mutation) {
        nameValue.SetText(mutation.mutatorDisplayData.name);
        mutationImage.sprite = mutation.sprite;
        descriptionValue.SetText(mutation.mutatorDisplayData.description);
        //
        // matteImage.color = mutation.archetype.color;
        // mutationImageMatte.color = (1f*mutation.archetype.color).WithAlpha(1f);
    }
}