using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Linq;
using Extensions;
using TMPro;

public class StatusBar : MonoBehaviour
{	
    [SerializeField] private GameObject panelContainer;
    private Unit boundUnit;

	void Awake() {
        // now, bind yourself to your parent Unit
        // just fail ungracefully if you don't have one, that shouldn't exist anyway
        boundUnit = GetComponentInParent<Unit>();
    }

    void Start() {
        // UpdateBar(boundUnit.unitStats.VITALITY, boundUnit.unitStats.VITALITY);
        //
        // boundUnit.unitStats.UpdateHPEvent += UpdateBar;
        // boundUnit.unitStats.UpdateDefenseEvent += UpdateArmor;
    }

    private void UpdateBar(int val, int max) {
        ///
        // update the levels appropriately
        ///
        // gather all segments
        // List<GameObject> segments = new List<GameObject>();
        // foreach (Transform bar in levelContainer.transform) {
        //     segments.Add(bar.gameObject);
        // }

        // // if there are too many segments:
        // while (segments.Count > max) {
        //     GameObject toDestroy = segments[segments.Count - 1];
        //     segments.Remove(toDestroy);
        //     Destroy(toDestroy);
        // }
        // // if there are too few segments:
        // while (segments.Count < max) {
        //     GameObject segment = Instantiate(barSegmentPrefab, levelContainer.transform);
        //     segments.Add(segment);
        // }
    }
}
