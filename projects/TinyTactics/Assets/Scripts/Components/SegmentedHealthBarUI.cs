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
    public Color color_0;
    public Color color_1;
    public Color dimColor;

    public GameObject healthContainer;
    public GameObject healthLevelContainer;
    public GameObject barSegmentPrefab;
    public TextMeshProUGUI healthValue;

    public GameObject armorContainer;
    public GameObject armorLevelContainer;
    public GameObject armorSegmentPrefab;
    public TextMeshProUGUI armorValue;


    private int currVal;
    private int maxVal;
    private float healthRatio;

    private Color barColor;

    public void AttachTo(Unit thisUnit) {
        UpdateBar(thisUnit.unitStats.VITALITY, thisUnit.unitStats.VITALITY);
        UpdateArmor(thisUnit.unitStats.DEFENSE);
        //
        thisUnit.unitStats.UpdateHPEvent += UpdateBar;
        thisUnit.unitStats.UpdateDefenseEvent += UpdateArmor;
    }

    private void UpdateBar(int val, int max) {
        bool healing = val > currVal;

        currVal = val;
        maxVal = max;
        healthRatio = (float)currVal/(float)maxVal;
        Vector3 toScale = new Vector3(healthRatio, 1.0f, 1.0f);

        ///
        // update the levels appropriately
        ///
        // gather all segments
        List<GameObject> segments = new List<GameObject>();
        foreach (Transform bar in healthLevelContainer.transform) {
            segments.Add(bar.gameObject);
        }

        // if there are too many segments:
        while (segments.Count > max) {
            GameObject toDestroy = segments[segments.Count - 1];
            segments.Remove(toDestroy);
            Destroy(toDestroy);
        }
        // if there are too few segments:
        while (segments.Count < max) {
            GameObject segment = Instantiate(barSegmentPrefab, healthLevelContainer.transform);
            segments.Add(segment);
        }

        // color the segments appropriately
        Color barColor = HueSatLerp(color_0, color_1, healthRatio*healthRatio);
        for (int l = 0; l < max; l++) {
            if (l < val)  segments[l].GetComponent<Image>().color = barColor;
            if (l >= val) segments[l].GetComponent<Image>().color = dimColor;
        }

        healthValue.SetText(currVal.ToString());
    }

    private void UpdateArmor(int defValue) {
        armorContainer.SetActive(defValue > 0);
        if (defValue < 1) return;
        //

        // gather all segments
        List<GameObject> segments = new List<GameObject>();
        foreach (Transform armor in armorLevelContainer.transform) {
            segments.Add(armor.gameObject);
        }

        // if there are too many segments:
        while (segments.Count > defValue) {
            GameObject toDestroy = segments[segments.Count - 1];
            segments.Remove(toDestroy);
            Destroy(toDestroy);
        }
        // if there are too few segments:
        while (segments.Count < defValue) {
            GameObject segment = Instantiate(armorSegmentPrefab, armorLevelContainer.transform);
            segments.Add(segment);
        }
        
        armorValue.SetText(defValue.ToString());
    }

    private static Color HueSatLerp(Color A, Color B, float ratio) {
        float AH, AS, AV, BH, BS, BV;
        Color.RGBToHSV(A, out AH, out AS, out AV);
        Color.RGBToHSV(B, out BH, out BS, out BV);

        float _H = Mathf.Lerp(AH, BH, ratio);
        float _S = Mathf.Lerp(AS, BS, ratio);
        return Color.HSVToRGB(_H, _S, 1f);
    }
}
