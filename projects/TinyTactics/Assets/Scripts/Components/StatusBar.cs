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
    [SerializeField] private StatusVisual statusVisualPrefab;
    private Unit boundUnit;

	void Awake() {
        // now, bind yourself to your parent Unit
        // just fail ungracefully if you don't have one, that shouldn't exist anyway
        boundUnit = GetComponentInParent<Unit>();
    }

    void Start() {
        boundUnit.statusSystem.AddStatusEvent += _ => UpdateBar();
        boundUnit.statusSystem.RemoveStatusEvent += _ => UpdateBar();

        CheckHide();
    }

    private void CheckHide() {
        GetComponent<CanvasGroup>().alpha = (boundUnit.statusSystem.Statuses.ToList().Count > 0) ? 1f : 0f;
    }

    private void ClearBar() {
		foreach (Transform t in panelContainer.transform) {
			Destroy(t.gameObject);
		}
    }

    private void UpdateBar() {
        ClearBar();

        foreach (so_Status status in boundUnit.statusSystem.Statuses) {
            StatusVisual sv = Instantiate(statusVisualPrefab, panelContainer.transform);
            sv.SetImage(status.sprite);
        }

        CheckHide();
    }
}
