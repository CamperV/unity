using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Linq;
using Extensions;
using TMPro;

public class BreakBarUI : MonoBehaviour
{	
    [SerializeField] private GameObject barSegmentPrefab;

    [SerializeField] private Color color_0;
    [SerializeField] private Color color_1;
    [SerializeField] private Color dimColor;

    private int currVal_Break;
    private int maxVal_Break;
    private float breakRatio;

    private List<GameObject> barSegments;
    
    [SerializeField] private GameObject container;
    [SerializeField] private TextMeshProUGUI breakValue;

    void Awake() {
        barSegments = new List<GameObject>();
    }

    public void AttachTo(Unit thisUnit) {
        UpdateAndRedraw(thisUnit.unitStats._CURRENT_BREAK, thisUnit.unitStats.BRAWN);
        //
        thisUnit.unitStats.UpdateBreakEvent += UpdateAndRedraw;
    }

    private void UpdateAndRedraw(int val, int max) {
        currVal_Break = val;
        maxVal_Break = max;
        breakRatio = (float)val/(float)max;

        Clear();
        barSegments.Clear();

        for (int s = 0; s < maxVal_Break; s++) {
            GameObject seg = Instantiate(barSegmentPrefab, container.transform);
            barSegments.Add(seg);
        }

        // color the break segments appropriately
        Color barColor = RatioColor(breakRatio);
        for (int l = 0; l < maxVal_Break; l++) {
            barSegments[l].GetComponent<Image>().color = (l < currVal_Break) ? barColor : dimColor;
        }

        // set Break value in text
        breakValue?.SetText($"{currVal_Break}");
    }

    public void Clear() {
        StopAllCoroutines();
        barSegments.Clear();

        foreach (Transform bar in container.transform) {
            Destroy(bar.gameObject);
        }

        breakValue?.SetText($"");
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
        return HueSatLerp(color_0, color_1, ratio*ratio);
    }
}
