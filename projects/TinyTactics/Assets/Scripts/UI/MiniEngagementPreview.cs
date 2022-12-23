using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniEngagementPreview : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damagePreview;
	[SerializeField] private GameObject arrowHeadPrefab;
	[SerializeField] private GameObject arrowBodyPrefab;

	private List<GameObject> arrowGameObjects;

	void Awake() {
		arrowGameObjects = new List<GameObject>();
	}

	public void DrawPreviewArrow(Vector3 from, Vector3 to) {
		Debug.DrawLine(from, to, color: Color.red, 5, false);
	}

	public void DrawPreviewArrow(Transform from, Transform to) {
		GameObject arrowHead = Instantiate(arrowHeadPrefab, transform);
		GameObject arrowBody = Instantiate(arrowBodyPrefab, transform);
		arrowGameObjects.Add(arrowHead);
		arrowGameObjects.Add(arrowBody);

		arrowHead.GetComponent<UIAnchor>().AnchorTo(to);
		arrowHead.GetComponent<UIAnchor>().AnchorRotationTowards(from);

		arrowBody.GetComponent<UIAnchor>().AnchorTo(from);
		arrowBody.GetComponent<UIAnchor>().AnchorRotationTowards(to);
		arrowBody.GetComponent<UIAnchor>().ScaleTowards(to);
	}

	public void SetEngagementStats(EngagementStats previewStats, int numStrikes) {
		Clear();

		if (!previewStats.Empty) {
			int min = previewStats.finalDamageContext.Min * numStrikes;
			int max = previewStats.finalDamageContext.Max * numStrikes;

			string damage = $"{min}";
			if (min != max) {
				damage += $" - {max}";
			}
			damagePreview.SetText($"<bounce>{damage}</bounce>");
		}
	}

	private void Clear() {
		damagePreview.SetText("");

		foreach (GameObject go in arrowGameObjects) {
			Destroy(go);
		}
	}
}