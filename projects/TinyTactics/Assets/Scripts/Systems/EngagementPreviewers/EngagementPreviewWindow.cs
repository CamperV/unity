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
		PopulatePanels(potentialEngagement);
		PopulatePanels(potentialEngagement, isCounter: true);

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

	private void PopulatePanels(Engagement potentialEngagement, bool isCounter = false) {
		GameObject panelToInstantiate;
		GameObject container;
		int numStrikes;

		if (isCounter == false) {
			panelToInstantiate = panelPrefab;
			container = panelContainer;
			numStrikes = potentialEngagement.A.statSystem.MULTISTRIKE+1;
		} else {
			panelToInstantiate = panelPrefab_Counter;
			container = panelContainer_Counter;
			numStrikes = potentialEngagement.B.statSystem.MULTISTRIKE+1;
		}

		// attack/counter
		string title = (isCounter == false) ? "Attack" : "Counterattack";
		if (numStrikes > 1) title += $" (x{numStrikes})".RichTextTags_TMP(color: "FFC27A");
		CreateAndSet($"{title}".RichTextTags_TMP(bold: true, fontSize: 20), panelToInstantiate, container);

		List<Attack> attacks = (isCounter == false) ? potentialEngagement.attacks : potentialEngagement.counterAttacks;
		foreach (Attack attack in attacks) {
			string damage = $"{attack.damage}".RichTextTags_TMP(color: "FFC27A");
			CreateAndSet($"{damage} dmg".RichTextTags_TMP(bold: true), panelToInstantiate, container);

			// if there's crit, set that too
			if (attack.critRate > 0) {
				string critical = $"{attack.critRate}%".RichTextTags_TMP(color: "FFC27A");
				CreateAndSet($"{critical} crit".RichTextTags_TMP(bold: true), panelToInstantiate, container);
			}
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

		foreach (MutatorDisplayData mut in mutators) {
			yield return mut;
		}
	}

	private IEnumerable<MutatorDisplayData> AllMutators(Engagement potentialEngagement) {
		foreach (var mut in BuildMutatorList(potentialEngagement, isCounter: false)) yield return mut;
		foreach (var mut in BuildMutatorList(potentialEngagement, isCounter: true)) yield return mut;
	}
}