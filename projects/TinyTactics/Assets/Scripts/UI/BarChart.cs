using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarChart : MonoBehaviour
{
    [SerializeField] private BarChartElement barChartElementPrefab;
    [SerializeField] private GameObject barContainer;
    
    public void Clear() {
        foreach (Transform t in barContainer.transform) {
            Destroy(t.gameObject);
        }
    }

    public void Visualize(Dictionary<int, float> xyData) {
        Clear();

        foreach (int x in xyData.Keys) {
            float y = xyData[x];
            BarChartElement bar = Instantiate(barChartElementPrefab, barContainer.transform);
            bar.SetLabel(x);
            bar.SetScale(y);
            Debug.Log($"Set {x} to scale {y}");
        }
    }
}
