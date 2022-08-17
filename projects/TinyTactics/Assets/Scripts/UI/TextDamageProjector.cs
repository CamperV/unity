using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextDamageProjector : UIDamageProjector
{
    [SerializeField] private TextMeshProUGUI text;

    public override void DisplayDamageProjection(EngagementStats engagementProjection) {
        Clear();

        if (!engagementProjection.Empty) {
            int min = engagementProjection.finalDamageContext.Min;
            int max = engagementProjection.finalDamageContext.Max;

            if (min != max) {
                text.SetText($"{min} - {max}");
            } else {
                text.SetText($"{min}");
            }
        }
    }

    private void Clear() {
        text.SetText($"--");
    }
}
