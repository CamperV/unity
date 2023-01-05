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

	// give the default size (RectTransform units) for when the preview is small/large
	// a preview is only large when a description is placed into it
	[SerializeField] private int smallWidth = 200;
	[SerializeField] private int largeWidth = 300;

	public void SetEngagementStats(Engagement potentialEngagement) {
		Clear();

		EngagementStats playerPreviewStats = potentialEngagement.SimulateAttack();
		EngagementStats enemyPreviewStats = potentialEngagement.SimulateCounterAttack();

		// then create panels and populate info
		PopulatePanels(potentialEngagement, playerPreviewStats);
		PopulatePanels(potentialEngagement, enemyPreviewStats, isCounter: true);

		// resize based on mutator descriptions
		ResizePanels(potentialEngagement);

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

		// attack/counter
		string title = (isCounter == false) ? "Attack" : "Counterattack";
		if (numStrikes > 1) title += $" (x{numStrikes})".RichTextTags_TMP(color: "FFC27A");
		CreateAndSet($"{title}".RichTextTags_TMP(bold: true, fontSize: 20), panelToInstantiate, container);

		// damage setting
		int min = previewStats.finalDamageContext.Min;
		int max = previewStats.finalDamageContext.Max;
		string damage = $"{min}";
		if (min != max) damage += $" - {max}";

		damage = damage.RichTextTags_TMP(color: "FFC27A");
		for (int n = 0; n < numStrikes; n++) {
			CreateAndSet($"{damage} dmg".RichTextTags_TMP(bold: true), panelToInstantiate, container);
		}

		// if there's crit, set that too
		if (previewStats.critRate > 0) {
			string critical = $"{previewStats.critRate}%".RichTextTags_TMP(color: "FFC27A");
			CreateAndSet($"{critical} crit".RichTextTags_TMP(bold: true), panelToInstantiate, container);
		}

		// also any attack mutators and their values
		foreach (MutatorDisplayData mutator in BuildMutatorList(potentialEngagement, isCounter: isCounter)) {
			string message = $"{mutator.name.RichTextTags_TMP(bold: true)}";
			if (mutator.description != "") {
				message += $"\n{mutator.description.RichTextTags_TMP(italics: true)}";
			}
			CreateAndSet(message, panelToInstantiate, container);
		}
	}

	private void CreateAndSet(string message, GameObject prefab, GameObject container) {
		GameObject panel = Instantiate(prefab, container.transform);
		panel.GetComponentInChildren<TextMeshProUGUI>().SetText(message);
	}

	private void ResizePanels(Engagement potentialEngagement) {
		RectTransform mainPanel = GetComponent<RectTransform>();

		bool requireLarge = false;
		foreach (MutatorDisplayData mutator in AllMutators(potentialEngagement)) {
			if (mutator.description != "") requireLarge = true;
		}
		int panelWidth = (requireLarge) ? largeWidth : smallWidth;
		
		// and resize if necessary
		mainPanel.sizeDelta = new Vector2(panelWidth, mainPanel.sizeDelta.y);
	}

	private IEnumerable<MutatorDisplayData> BuildMutatorList(Engagement potentialEngagement, bool isCounter = false) {
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

	private IEnumerable<MutatorDisplayData> AllMutators(Engagement potentialEngagement) {
		foreach (var mut in BuildMutatorList(potentialEngagement, isCounter: false)) yield return mut;
		foreach (var mut in BuildMutatorList(potentialEngagement, isCounter: true)) yield return mut;
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