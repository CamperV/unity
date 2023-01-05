using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Extensions;

public class EngagementPreviewWindow : MonoBehaviour
{
    [SerializeField] private GameObject panelPrefab;
	[SerializeField] private GameObject panelPrefab_Counter;

	[SerializeField] private GameObject panelContainer;
	[SerializeField] private GameObject panelContainer_Counter;

	public void SetEngagementStats(Engagement potentialEngagement) {
		EngagementStats playerPreviewStats = potentialEngagement.SimulateAttack();
		EngagementStats enemyPreviewStats = potentialEngagement.SimulateCounterAttack();

		Clear();
		PopulatePanels(potentialEngagement, playerPreviewStats);
		PopulatePanels(potentialEngagement, enemyPreviewStats, isCounter: true);

		// finally, deal with Unity wonkiness
		foreach (ContentSizeFitter csf in GetComponentsInChildren<ContentSizeFitter>()) {
			LayoutRebuilder.ForceRebuildLayoutImmediate(csf.GetComponent<RectTransform>());
		}
	}

	private void Clear() {
		foreach (Transform child in panelContainer.transform) {
			Destroy(child.gameObject);
		}
		foreach (Transform child in panelContainer_Counter.transform) {
			Destroy(child.gameObject);
		}
	}

	private void PopulatePanels(Engagement potentialEngagement, EngagementStats previewStats, bool isCounter = false) {
		GameObject panelToInstantiate;
		GameObject container;
		int numStrikes;

		if (isCounter == false) {
			panelToInstantiate = panelPrefab;
			container = panelContainer;
			numStrikes = potentialEngagement.aggressor.statSystem.MULTISTRIKE+1;
		} else {
			panelToInstantiate = panelPrefab_Counter;
			container = panelContainer_Counter;
			numStrikes = potentialEngagement.defender.statSystem.MULTISTRIKE+1;
		}

		int min = previewStats.finalDamageContext.Min * numStrikes;
		int max = previewStats.finalDamageContext.Max * numStrikes;

		string damage = $"{min}";
		if (min != max) damage += $" - {max}";
		CreateAndSet($"{damage} damage", panelToInstantiate, container);

		// if there's crit, set that too
		if (previewStats.critRate > 0) {
			CreateAndSet($"{previewStats.critRate}% critical", panelToInstantiate, container);
		}

		// also any attack mutators and their values
		foreach (MutatorDisplayData mutator in BuildMutatorList(potentialEngagement, previewStats, isCounter: isCounter)) {
			string message = $"{mutator.name.RichTextTags(bold: true)}";
			if (mutator.description != "") message += $"\n{mutator.description.RichTextTags(italics: true)}";
			CreateAndSet(message, panelToInstantiate, container);
		}
	}

	private void CreateAndSet(string message, GameObject prefab, GameObject container) {
		GameObject panel = Instantiate(prefab, container.transform);
		panel.GetComponentInChildren<TextMeshProUGUI>().SetText(message);
	}

	private IEnumerable<MutatorDisplayData> BuildMutatorList(Engagement potentialEngagement, EngagementStats previewStats, bool isCounter = false) {
		List<MutatorDisplayData> mutators = new List<MutatorDisplayData>();

		if (isCounter == false) {
			mutators = potentialEngagement.attack.mutators;
			if (potentialEngagement.counterDefense != null) {
				mutators = mutators.Concat(potentialEngagement.counterDefense.Value.mutators).ToList();
			}

		} else {
			mutators = potentialEngagement.defense.mutators;
			if (potentialEngagement.counterAttack != null) {
				mutators = mutators.Concat(potentialEngagement.counterAttack.Value.mutators).ToList();
			}
		}

		foreach (MutatorDisplayData mut in mutators) {
			yield return mut;
		}
	}

	// private List<T> GetComponentsInChildrenBFS<T>(GameObject root) {
	// 	List<T> retval = new List<T>();
	// 	List<Transform> queue = new List<Transform>{root.transform};

	// 	while (queue.Count > 0) {
	// 		Transform current = queue.PopAt(0);

	// 		T comp = current.gameObject.GetComponent<T>();
	// 		if (comp != null) retval.Add(comp);

	// 		// all immediate children
	// 		foreach (Transform child in current) {
	// 			queue.Add(child);
	// 		}
	// 	}

	// 	return retval;
	// }
}