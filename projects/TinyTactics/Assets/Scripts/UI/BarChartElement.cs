using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarChartElement : MonoBehaviour
{
    [SerializeField] private Image barImage;
    [SerializeField] private TextMeshProUGUI label;

    public void SetScale(float scale) {
        Debug.Assert(scale >= 0f && scale <= 1f);
        barImage.fillAmount = scale;
    }

    public void SetLabel(int value) {
        label.SetText($"{value}");
    }
}
