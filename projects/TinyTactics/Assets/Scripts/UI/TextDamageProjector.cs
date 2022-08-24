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

            string accum = "";
            if (min != max) {
                accum += $"{min} - {max}";
            } else {
                accum += $"{min}";
            }
            
            if (engagementProjection.critRate > 0) {
                accum += $" [{engagementProjection.critRate}]";
            }
            text.SetText(accum);
        }
    }

    private void Clear() {
        text.SetText($"--");
    }
}
