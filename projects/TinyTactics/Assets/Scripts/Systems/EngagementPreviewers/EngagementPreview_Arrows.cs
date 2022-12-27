using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EngagementPreview_Arrows : MonoBehaviour, IEngagementPreviewer
{
	public Canvas targetCanvas;
	[SerializeField] private GameObject arrowHeadPrefab;
	[SerializeField] private GameObject arrowBodyPrefab;
	[SerializeField] private GameObject arrowBody_ComboPrefab;
	[SerializeField] private GameObject arrowBody_CounterPrefab;

	public void EnablePreview(Engagement potentialEngagement) {
		DrawPreviewArrow(
			potentialEngagement.aggressor.transform,
			potentialEngagement.defender.transform,
			arrowBodyPrefab,
			// shiftRight: potentialEngagement.counterAttack != null
			shiftRight: false
		);
		foreach (ComboAttack combo in potentialEngagement.comboAttacks) {
			DrawPreviewArrow(
				combo.unit.transform,
				potentialEngagement.defender.transform,
				arrowBody_ComboPrefab
			);
		}

		// and the counter-arrow
		// counter-attack arrow
		// shift it right to look alright
		// DrawPreviewArrow(
		// 	potentialEngagement.defender.transform,
		// 	potentialEngagement.aggressor.transform,
		// 	arrowBody_CounterPrefab,
		// 	shiftRight: true
		// );
	}

	public void DisablePreview(Engagement potentialEngagement) {
		foreach (Transform arrow in transform) {
			Destroy(arrow.gameObject);
		}
	}

	private void DrawPreviewArrow(Transform from, Transform to, GameObject prefab, bool shiftRight = false) {
		GameObject arrowBody = Instantiate(prefab, transform);

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
}