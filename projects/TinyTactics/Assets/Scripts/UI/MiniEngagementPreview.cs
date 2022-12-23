using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniEngagementPreview : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damagePreview;
	[SerializeField] private GameObject arrowContainer;
	[SerializeField] private GameObject arrowHeadPrefab;
	[SerializeField] private GameObject arrowBodyPrefab;
	[SerializeField] private GameObject arrowBody_ComboPrefab;
	[SerializeField] private GameObject arrowBody_CounterPrefab;

	private List<GameObject> arrowGameObjects;

	void Awake() {
		arrowGameObjects = new List<GameObject>();
	}

	private void DrawPreviewArrow(Transform from, Transform to, GameObject prefab, bool shiftRight = false) {
		GameObject arrowBody = Instantiate(prefab, arrowContainer.transform);
		arrowGameObjects.Add(arrowBody);

		UIAnchor arrowAnchor = arrowBody.GetComponent<UIAnchor>();
		arrowAnchor.AnchorTo(from);
		arrowAnchor.AnchorRotationTowards(to);

		// float offset = arrowHead.GetComponent<RectTransform>().sizeDelta.y;
		float offset = 35f;
		arrowAnchor.ScaleTowards(to, sizeDeltaOffset: offset);

		// finally, if you are simulating an engagement with a counterAttack,
		// shift the main arrows slightly to make room for one another
		// shift should be in world units
		if (shiftRight) {
			Vector3 relativeRotationVector = (to.position - from.position);
			relativeRotationVector.Normalize();

			// -y, x is 90 deg rotation widdershins
			Vector3 relativeLeft = new Vector3(
				-relativeRotationVector.y,
				relativeRotationVector.x,
				0
			);
			arrowAnchor.ShiftAnchorOffset(relativeLeft, 0.05f, shiftRotationAnchor: true);
		}
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
			damagePreview.SetText($"<wave>{damage}</wave>");
		}
	}

	public void SetEngagementStats(Engagement potentialEngagement, EngagementStats previewStats, bool isAggressor) {
		Clear();

		int numStrikes = (isAggressor) ? potentialEngagement.aggressor.statSystem.MULTISTRIKE+1 : potentialEngagement.aggressor.statSystem.MULTISTRIKE+1;

		// set damage value
		if (!previewStats.Empty) {
			int min = previewStats.finalDamageContext.Min * numStrikes;
			int max = previewStats.finalDamageContext.Max * numStrikes;

			string damage = $"{min}";
			if (min != max) {
				damage += $" - {max}";
			}

			// aka can you receive these combo attacks
			if (isAggressor == false) {
				foreach (ComboAttack combo in potentialEngagement.comboAttacks) {
					damage += $"<size=20> \n+{combo.damage}</size>";
				}
			}

			damagePreview.SetText($"<bounce>{damage}</bounce>");
		}

		// and draw arrows when you're the aggressor
		if (isAggressor) {
			DrawPreviewArrow(
				potentialEngagement.aggressor.transform,
				potentialEngagement.defender.transform,
				arrowBodyPrefab,
				shiftRight: potentialEngagement.counterAttack != null
			);
			foreach (ComboAttack combo in potentialEngagement.comboAttacks) {
				DrawPreviewArrow(
					combo.unit.transform,
					potentialEngagement.defender.transform,
					arrowBody_ComboPrefab
				);
			}
		} else {
			DrawPreviewArrow(
				potentialEngagement.defender.transform,
				potentialEngagement.aggressor.transform,
				arrowBody_CounterPrefab,
				shiftRight: true
			);
		}
	}

	private void Clear() {
		damagePreview.SetText("");

		foreach (GameObject go in arrowGameObjects) {
			Destroy(go);
		}
	}
}