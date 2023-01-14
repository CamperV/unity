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
		foreach (Unit target in potentialEngagement.targets) {
			DrawPreviewArrow(
				potentialEngagement.initiator.transform,
				target.transform,
				arrowBodyPrefab,
				// shiftRight: potentialEngagement.counterAttack != null
				shiftRight: false
			);
		}

	}

	public void DisablePreview(Engagement _) {
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