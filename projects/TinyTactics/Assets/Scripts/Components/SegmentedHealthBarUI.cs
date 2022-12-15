using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Linq;
using Extensions;
using TMPro;

public class SegmentedHealthBarUI : MonoBehaviour
{	
    // this dict contains a float non-linear step to Lerp between for health::color relationships
    // ie, less than 5 is red, more than 15 is blue-green, etc
    // this is NOT a 0 - 100% scale, so that any full-health bar is the same color (even if one has 1 HP and one has 100 HP)
    // this would remain red even if at full-health, if around the threshold
    [SerializeField] private Color color_0;
    [SerializeField] private Color color_1;
    [SerializeField] private Color dimColor;
    [SerializeField] private Color threatenedColor;

    [SerializeField] private bool useRatioColor = true;

    [SerializeField] private GameObject combinedLevelContainer;
    [SerializeField] private TextMeshProUGUI combinedPreviewValue;

    // optional
    [SerializeField] private TextMeshProUGUI hpValue;
    [SerializeField] private TextMeshProUGUI drValue;

    [SerializeField] private GameObject barSegmentPrefab;
    [SerializeField] private GameObject armorSegmentPrefab;

    private int currVal_Health;
    private int maxVal_Health;
    private float healthRatio;
    private List<GameObject> healthSegments;
    private int currVal_Armor;

    private Color barColor;

    // don't love this, but the best way to clear for right now
    private Unit? attachedUnit;

    void Awake() {
        healthSegments = new List<GameObject>();
    }

    public void AttachTo(Unit thisUnit) {
        UpdateHealthAndRedraw(thisUnit.unitStats._CURRENT_HP, thisUnit.unitStats.VITALITY);
        UpdateArmorAndRedraw(thisUnit.unitStats.DEFENSE);
        //
        thisUnit.unitStats.UpdateHPEvent += UpdateHealthAndRedraw;
        thisUnit.unitStats.UpdateDefenseEvent += UpdateArmorAndRedraw;

        attachedUnit = thisUnit;
    }

    public void Detach() {
        if (attachedUnit != null) {
            attachedUnit.unitStats.UpdateHPEvent -= UpdateHealthAndRedraw;
            attachedUnit.unitStats.UpdateDefenseEvent -= UpdateArmorAndRedraw;
        }
        attachedUnit = null;
    }

    private void UpdateHealthAndRedraw(int val, int max) {
        currVal_Health = val;
        maxVal_Health = max;
        healthRatio = (float)val/(float)max;
        UpdateAll();
    }

    private void UpdateArmorAndRedraw(int defValue) {
        currVal_Armor = defValue;
        UpdateAll();
    }

    public void Clear() {
        StopAllCoroutines();
        healthSegments.Clear();

        foreach (Transform bar in combinedLevelContainer.transform) {
            Destroy(bar.gameObject);
        }

        combinedPreviewValue?.SetText($"");
    }

    private void UpdateAll() {
        Clear();

        // health first
        healthSegments.Clear();
        for (int s = 0; s < maxVal_Health; s++) {
            GameObject seg = Instantiate(barSegmentPrefab, combinedLevelContainer.transform);
            healthSegments.Add(seg);
        }

        // color the health segments appropriately
        Color barColor = RatioColor(healthRatio);
        for (int l = 0; l < maxVal_Health; l++) {
            healthSegments[l].GetComponent<Image>().color = (l < currVal_Health) ? barColor : dimColor;
        }

        // set Health value in text
        combinedPreviewValue?.SetText($"<color=#05D97A>{currVal_Health}</color>");
        hpValue?.SetText($"{currVal_Health}");

        // now set armor, if you dare
        if (currVal_Armor > 0) {
            for (int a = 0; a < currVal_Armor; a++) {
                Instantiate(armorSegmentPrefab, combinedLevelContainer.transform);
            }

            combinedPreviewValue?.SetText($"{combinedPreviewValue.text} <color=#DE9E41>({currVal_Armor})</color>");
            drValue?.SetText($"{currVal_Armor}");
        }
    }

    public void PreviewDamage(int damageAmountPreview) {
        //
        // visually flash the bar to demonstrate the health loss
        //
        StartFlashSegments(damageAmountPreview);

        //
        // and also explicity show the new health value
        //
        Color currentColor = RatioColor(healthRatio);

        // now get the color that the bar will be
        int previewHealth = (int)Mathf.Max(0f, currVal_Health - damageAmountPreview);
        float previewRatio = (float)previewHealth/(float)maxVal_Health;
        Color previewColor = RatioColor(previewRatio);

        string currentColor_Hex = ColorUtility.ToHtmlStringRGB(currentColor);
        string previewColor_Hex = ColorUtility.ToHtmlStringRGB(previewColor);
        // combinedPreviewValue.SetText($"<color=#{currentColor_Hex}>{currVal_Health}</color>\u2192<color=#{previewColor_Hex}>{previewHealth}</color>");
        combinedPreviewValue?.SetText($"<color=#{currentColor_Hex}>{currVal_Health}</color>");
        hpValue?.SetText($"<color=#{currentColor_Hex}>{currVal_Health}</color>");
    }

    private void StartFlashSegments(int numSegmentsFromBack) {
        if (healthSegments.Count > 0) {
            int healthDiff = maxVal_Health - currVal_Health;
            StartCoroutine(
                FlashSegments(healthSegments.AsEnumerable().Reverse().Skip(healthDiff).Take(numSegmentsFromBack))
            );
        }
    }

    private IEnumerator FlashSegments(IEnumerable<GameObject> barSegments) {
        foreach (GameObject barSegment in barSegments) {
            StartCoroutine( FlashSingle(barSegment) );
            // yield return null;
            // yield return new WaitForSeconds(0.1f);
        }
        yield break;
    }

    private IEnumerator FlashSingle(GameObject barSegment) {
        Image image = barSegment.GetComponent<Image>();
        Color originalColor = image.color;

        float fixedTime = 0.5f;
		float timeRatio = 0.0f;

        while (true) {
            // to dim
            timeRatio = 0.0f;
            while (timeRatio < 1.0f) {
                timeRatio += (Time.deltaTime / fixedTime);
                image.color = Color.Lerp(originalColor, threatenedColor, timeRatio).WithAlpha(1f);
                yield return null;
            }
            // yield return new WaitForSeconds(0.25f);

            // and then back up to normal color
            timeRatio = 0.0f;
            while (timeRatio < 1.0f) {
                timeRatio += (Time.deltaTime / fixedTime);
                image.color = Color.Lerp(threatenedColor, originalColor, timeRatio).WithAlpha(1f);
                yield return null;
            }

            // wait on "normal color"
            // yield return new WaitForSeconds(0.50f);
        }
    }
    
    private static Color HueSatLerp(Color A, Color B, float ratio) {
        float AH, AS, AV, BH, BS, BV;
        Color.RGBToHSV(A, out AH, out AS, out AV);
        Color.RGBToHSV(B, out BH, out BS, out BV);

        float _H = Mathf.Lerp(AH, BH, ratio);
        float _S = Mathf.Lerp(AS, BS, ratio);
        return Color.HSVToRGB(_H, _S, 1f);
    }

    private Color RatioColor(float ratio) {
        return (useRatioColor) ? HueSatLerp(color_0, color_1, ratio*ratio) : color_0;
    }
}
