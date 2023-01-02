using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EngagementPreviewWindow : MonoBehaviour
{
    [SerializeField] private GameObject panelPrefab;
	[SerializeField] private GameObject panelPrefab_Counter;

	public void SetEngagementStats(Engagement potentialEngagement) {
		EngagementStats playerPreviewStats = potentialEngagement.SimulateAttack();
		EngagementStats enemyPreviewStats = potentialEngagement.SimulateCounterAttack();

		Clear();
		PopulatePanels(potentialEngagement, playerPreviewStats);
		PopulatePanels(potentialEngagement, enemyPreviewStats, isCounter: true);

		// finally, deal with Unity wonkiness
		foreach (ContentSizeFitter csf in GetComponentsInChildren<ContentSizeFitter>().Reverse()) {
			LayoutRebuilder.ForceRebuildLayoutImmediate(csf.GetComponent<RectTransform>());
		}
		// foreach (LayoutGroup layout in GetComponentsInChildren<LayoutGroup>()) {
		// 	layout.enabled = false;
		// 	layout.enabled = true;
		// }
		// Canvas.ForceUpdateCanvases();
	}

	private void Clear() {
		foreach (Transform child in transform) {
			Destroy(child.gameObject);
		}
	}

	private void PopulatePanels(Engagement potentialEngagement, EngagementStats previewStats, bool isCounter = false) {
		GameObject panelToInstantiate = (isCounter == false) ? panelPrefab : panelPrefab_Counter;
		
		// be mindful of multistrike
		//		for simulating: for each Attack, simulate
		int numStrikes = (isCounter == false) ? potentialEngagement.aggressor.statSystem.MULTISTRIKE+1 : potentialEngagement.defender.statSystem.MULTISTRIKE+1;
		int min = previewStats.finalDamageContext.Min * numStrikes;
		int max = previewStats.finalDamageContext.Max * numStrikes;

		string damage = $"{min}";
		if (min != max) damage += $" - {max}";
		CreateAndSet($"{damage} damage", panelToInstantiate);

		// if there's crit, set that too
		if (previewStats.critRate > 0) {
			CreateAndSet($"{previewStats.critRate}% critical", panelToInstantiate);
		}

		// also any attack mutators and their values
		foreach (string mutator in previewStats.mutators) {
			CreateAndSet(mutator, panelToInstantiate);
		}
	}

	private void CreateAndSet(string message, GameObject prefab) {
		GameObject panel = Instantiate(prefab, transform);
		panel.GetComponentInChildren<TextMeshProUGUI>().SetText(message);
	}
}