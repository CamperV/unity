using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Linq;
using Extensions;
using TMPro;

public class PoiseBar_UI : MonoBehaviour
{	
    [SerializeField] private GameObject barSegmentPrefab;

    [SerializeField] private Color color_0;
    [SerializeField] private Color color_1;
    [SerializeField] private Color dimColor;

    private int currVal_Poise;
    private int maxVal_Poise;
    private float poiseRatio;

    private List<GameObject> barSegments;
    
    [SerializeField] private GameObject container;
    [SerializeField] private TextMeshProUGUI poiseValue;

    void Awake() {
        barSegments = new List<GameObject>();
    }

    public void AttachTo(Unit thisUnit) {
        UpdateAndRedraw(thisUnit.statSystem.CURRENT_POISE, thisUnit.statSystem.MAX_POISE);
        //
        thisUnit.statSystem.UpdatePoiseEvent += UpdateAndRedraw;
    }

    private void UpdateAndRedraw(int val, int max) {
        currVal_Poise = val;
        maxVal_Poise = max;
        poiseRatio = (float)val/(float)max;

        Clear();
        barSegments.Clear();

        for (int s = 0; s < maxVal_Poise; s++) {
            GameObject seg = Instantiate(barSegmentPrefab, container.transform);
            barSegments.Add(seg);
        }

        // color the poise segments appropriately
        Color barColor = RatioColor(poiseRatio);
        for (int l = 0; l < maxVal_Poise; l++) {
            barSegments[l].GetComponent<Image>().color = (l < currVal_Poise) ? barColor : dimColor;
        }

        // set Poise value in text
        poiseValue?.SetText($"{currVal_Poise}");
    }

    public void Clear() {
        StopAllCoroutines();
        barSegments.Clear();

        foreach (Transform bar in container.transform) {
            Destroy(bar.gameObject);
        }

        poiseValue?.SetText($"");
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
