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

            accum = $"{accum}";
            if (multistrikeValue > 0) {
                accum += $" <wave><sup><size=64><color=#{msColor_Hex}>x{multistrikeValue+1}</color></size></sup></wave>";
            }
            damageText.SetText($"{accum}");
            
            // then, crit as well
            critText.SetText($"{engagementProjection.critRate}");
        }
    }

    public override void DisplayDamageProjection(Engagement potentialEngagement, EngagementStats engagementProjection, int multistrikeValue) {
        Clear();

        int comboDamage = 0;
        foreach (ComboAttack combo in potentialEngagement.comboAttacks) {
            comboDamage += Mathf.Clamp(combo.damage - potentialEngagement.defense.damageReduction, 0, 99);
        }

        if (!engagementProjection.Empty) {
            int min = engagementProjection.finalDamageContext.Min;
            int max = engagementProjection.finalDamageContext.Max;

            string accum = "";
            if (min != max) {
                accum += $"{min} - {max}";
            } else {
                accum += $"{min}";
            }

            if (comboDamage > 0) accum += $"<color=#{ColorUtility.ToHtmlStringRGB(x2_Color)}> + {comboDamage}</color>";

            string msColor_Hex = "";
            if (multistrikeValue == 1)      msColor_Hex = ColorUtility.ToHtmlStringRGB(x2_Color);
            else if (multistrikeValue == 2) msColor_Hex = ColorUtility.ToHtmlStringRGB(x3_Color);
            else if (multistrikeValue >= 3) msColor_Hex = ColorUtility.ToHtmlStringRGB(x4_Color);

            accum = $"{accum}";
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
