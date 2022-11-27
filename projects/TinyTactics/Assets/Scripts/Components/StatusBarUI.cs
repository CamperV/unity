using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Linq;
using Extensions;
using TMPro;

public class StatusBarUI : MonoBehaviour
{	
    [SerializeField] private GameObject panelContainer;
    [SerializeField] private StatusVisual statusVisualPrefab;

    // don't love this, but the best way to clear for right now
    private Unit? attachedUnit;

    public void AttachTo(Unit thisUnit) {
        thisUnit.statusSystem.AddStatusEvent += UpdateBarWithStatus;
        thisUnit.statusSystem.RemoveStatusEvent += UpdateBarWithStatus;
        attachedUnit = thisUnit;

        _UpdateBar();
        CheckHide();
    }

    public void Detach() {
        attachedUnit.statusSystem.AddStatusEvent -= UpdateBarWithStatus;
        attachedUnit.statusSystem.RemoveStatusEvent -= UpdateBarWithStatus;
        attachedUnit = null;
    }

    private void CheckHide() {
        GetComponent<CanvasGroup>().alpha = (attachedUnit.statusSystem.Statuses.ToList().Count > 0) ? 1f : 0f;
    }

    private void ClearBar() {
		foreach (Transform t in panelContainer.transform) {
			Destroy(t.gameObject);
		}
    }

    private void UpdateBarWithStatus(so_Status _) => _UpdateBar();
    private void _UpdateBar() {
        ClearBar();

        foreach (so_Status status in attachedUnit.statusSystem.Statuses) {
            StatusVisual sv = Instantiate(statusVisualPrefab, panelContainer.transform);
            sv.SetImage(status.sprite);
        }

        CheckHide();
    }
}
