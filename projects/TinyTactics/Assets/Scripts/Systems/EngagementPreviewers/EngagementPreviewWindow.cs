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

		// then create panels and populate info
		PopulateAttackPanels(potentialEngagement);
		if (potentialEngagement.counterAttacks.Count > 0)
			PopulateCounterattackPanels(potentialEngagement);

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

	private void PopulateAttackPanels(Engagement potentialEngagement) {
		GameObject panelToInstantiate = panelPrefab;
		GameObject container = panelContainer;
		int numStrikes = potentialEngagement.initiator.statSystem.MULTISTRIKE+1;

		// attack/counter
		string title = "Attack";
		if (numStrikes > 1) title += $" (x{numStrikes})".RichTextTags_TMP(color: "FFC27A");
		CreateAndSet($"{title}".RichTextTags_TMP(bold: true, fontSize: 20), panelToInstantiate, container);

		foreach (Attack attack in potentialEngagement.attacks) {
			string damage = $"{attack.damage}".RichTextTags_TMP(color: "FFC27A");
			CreateAndSet($"{damage} dmg".RichTextTags_TMP(bold: true), panelToInstantiate, container);

			// if there's crit, set that too
			if (attack.critRate > 0) {
				string critical = $"{attack.critRate}%".RichTextTags_TMP(color: "FFC27A");
				CreateAndSet($"{critical} crit".RichTextTags_TMP(bold: true), panelToInstantiate, container);
			}
		}

		// also any attack mutators and their values
		foreach (MutatorDisplayData mutator in BuildMutatorList(potentialEngagement)) {
			string message = $"{mutator.name.RichTextTags_TMP(bold: true)}";
			if (mutator.description != "") {
				message += $"\n{mutator.description.RichTextTags_TMP(italics: true)}";
			}
			CreateAndSet(message, panelToInstantiate, container);
		}
	}

	private void PopulateCounterattackPanels(Engagement potentialEngagement) {
		// only set up a preview with targets[0], ie the main target
		// too much work to preview every single target
		GameObject panelToInstantiate = panelPrefab_Counter;
		GameObject container = panelContainer_Counter;
		int numStrikes = potentialEngagement.targets[0].statSystem.MULTISTRIKE+1;


		// attack/counter
		string title = "Counterattack";
		if (numStrikes > 1) title += $" (x{numStrikes})".RichTextTags_TMP(color: "FFC27A");
		CreateAndSet($"{title}".RichTextTags_TMP(bold: true, fontSize: 20), panelToInstantiate, container);

		foreach (Attack attack in potentialEngagement.counterAttacks) {
			string damage = $"{attack.damage}".RichTextTags_TMP(color: "FFC27A");
			CreateAndSet($"{damage} dmg".RichTextTags_TMP(bold: true), panelToInstantiate, container);

			// if there's crit, set that too
			if (attack.critRate > 0) {
				string critical = $"{attack.critRate}%".RichTextTags_TMP(color: "FFC27A");
				CreateAndSet($"{critical} crit".RichTextTags_TMP(bold: true), panelToInstantiate, container);
			}
		}

		// also any attack mutators and their values
		foreach (MutatorDisplayData mutator in BuildMutatorList(potentialEngagement, isCounter: true)) {
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
			foreach (Attack attack in potentialEngagement.attacks) {
				foreach (MutatorDisplayData mdd in attack.attackMutators) {
					mutators.Add(mdd);
				}
			}
			foreach (Attack counterAttack in potentialEngagement.counterAttacks) {
				foreach (MutatorDisplayData mdd in counterAttack.defenseMutators) {
					mutators.Add(mdd);
				}
			}

		} else {
			foreach (Attack counterAttack in potentialEngagement.counterAttacks) {
				foreach (MutatorDisplayData mdd in counterAttack.attackMutators) {
					mutators.Add(mdd);
				}
			}
			foreach (Attack attack in potentialEngagement.attacks) {
				foreach (MutatorDisplayData mdd in attack.defenseMutators) {
					mutators.Add(mdd);
				}
			}
		}

		foreach (MutatorDisplayData mut in mutators.Distinct()) {
			yield return mut;
		}
	}

	private IEnumerable<MutatorDisplayData> AllMutators(Engagement potentialEngagement) {
		foreach (var mut in BuildMutatorList(potentialEngagement, isCounter: false)) yield return mut;
		foreach (var mut in BuildMutatorList(potentialEngagement, isCounter: true)) yield return mut;
	}
}