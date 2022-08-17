using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarChart : UIDamageProjector
{
    [SerializeField] private BarChartElement barChartElementPrefab;
    [SerializeField] private GameObject barContainer;

    public override void DisplayDamageProjection(EngagementStats engagementProjection) {
        Clear();

        if (!engagementProjection.Empty) {
            Dictionary<int, float> simResults = engagementProjection.finalDamageContext.Projection;
            
            foreach (int x in simResults.Keys) {
                float y = simResults[x];
                BarChartElement bar = Instantiate(barChartElementPrefab, barContainer.transform);
                bar.SetLabel(x);
                bar.SetScale(y);
                Debug.Log($"Set {x} to scale {y}");
            }
        }
    }

    private void Clear() {
        foreach (Transform t in barContainer.transform) {
            Destroy(t.gameObject);
        }
    }
}
