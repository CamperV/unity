using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextDamageProjector : UIDamageProjector
{
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI critText;

    // for multistrike
    [SerializeField] private Color x2_Color;
    [SerializeField] private Color x3_Color;
    [SerializeField] private Color x4_Color;

    public override void DisplayDamageProjection(EngagementStats engagementProjection) {
        Clear();

        if (!engagementProjection.Empty) {
            int min = engagementProjection.finalDamageContext.Min;
            int max = engagementProjection.finalDamageContext.Max;

            if (min != max) {
                damageText.SetText($"<wave>{min} - {max}</wave>");
            } else {
                damageText.SetText($"<wave>{min}</wave>");
            }
            
            // then, crit as well
            critText.SetText($"{engagementProjection.critRate}");
        }
    }

    public override void DisplayDamageProjection(EngagementStats engagementProjection, int multistrikeValue) {
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

            string msColor_Hex = "";
            if (multistrikeValue == 1)      msColor_Hex = ColorUtility.ToHtmlStringRGB(x2_Color);
            else if (multistrikeValue == 2) msColor_Hex = ColorUtility.ToHtmlStringRGB(x3_Color);
            else if (multistrikeValue >= 3) msColor_Hex = ColorUtility.ToHtmlStringRGB(x4_Color);

            if (multistrikeValue > 0) {
                accum += $" <wave><sup><size=64><color=#{msColor_Hex}>x{multistrikeValue+1}</color></size></sup></wave>";
            }
            damageText.SetText($"{accum}");
            
            // then, crit as well
            critText.SetText($"{engagementProjection.critRate}");
        }
    }

    private void Clear() {
        damageText.SetText($"--");
        critText.SetText($"--");
    }
}
