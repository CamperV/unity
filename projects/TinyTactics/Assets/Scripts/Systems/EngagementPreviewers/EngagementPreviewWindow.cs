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
		CreateTitle("Attack", potentialEngagement.initiator, panelPrefab, panelContainer);
		CreateSinglePanel_Attacks(Attack.AttackDirection.Normal, potentialEngagement, panelPrefab, panelContainer);
		CreatePanels_Mutations(Attack.AttackDirection.Normal, potentialEngagement, panelPrefab, panelContainer);
	}


	// only set up a preview with targets[0], ie the main target
	// too much work to preview every single target
	private void PopulateCounterattackPanels(Engagement potentialEngagement) {
		CreateTitle("Counterattack", potentialEngagement.targets[0], panelPrefab_Counter, panelContainer_Counter);
		CreateSinglePanel_Attacks(Attack.AttackDirection.Counter, potentialEngagement, panelPrefab_Counter, panelContainer_Counter);
		CreatePanels_Mutations(Attack.AttackDirection.Counter, potentialEngagement, panelPrefab_Counter, panelContainer_Counter);
	}

	private void CreateTitle(string _title, Unit unit, GameObject panel, GameObject container) {
		string title = _title;
		if (unit.statSystem.MULTISTRIKE > 0)
			title += $" (x{unit.statSystem.MULTISTRIKE+1})".RichTextTags_TMP(color: "FFC27A");
		CreateAndSet($"{title}".RichTextTags_TMP(bold: true, fontSize: 20), panel, container);
	}

	private void CreatePanels_Attacks(Attack.AttackDirection attackDirection, Engagement potentialEngagement, GameObject panel, GameObject container) {
		foreach (Attack attack in potentialEngagement.GetAttacks()) {
			if (attack.attackDirection != attackDirection) continue;

			string damage = $"{attack.damage}".RichTextTags_TMP(color: "FFC27A");
			CreateAndSet($"{damage} dmg".RichTextTags_TMP(bold: true), panel, container);

			// if there's crit, set that too
			if (attack.critRate > 0) {
				string critical = $"{attack.critRate}%".RichTextTags_TMP(color: "FFC27A");
				CreateAndSet($"{critical} crit".RichTextTags_TMP(bold: true), panel, container);
			}
		}
	}

	private void CreateSinglePanel_Attacks(Attack.AttackDirection attackDirection, Engagement potentialEngagement, GameObject panel, GameObject container) {
		List<string> damageCollector = new List<string>();

		foreach (Attack attack in potentialEngagement.GetAttacks()) {
			if (attack.attackDirection != attackDirection) continue;

			string damage = $"{attack.damage}".RichTextTags_TMP(color: "FFC27A");
			damage = $"{damage} dmg".RichTextTags_TMP(bold: true);

			// if there's crit, set that too
			if (attack.critRate > 0) {
				string critical = $"{attack.critRate}%".RichTextTags_TMP(color: "FFC27A");
				critical = $"{critical} crit".RichTextTags_TMP(bold: true);
				damage = $"{damage} [{critical}]";
			}
			
			damageCollector.Add(damage);
		}

		CreateAndSet(string.Join("\n", damageCollector), panel, container);
	}

	private void CreatePanels_Mutations(Attack.AttackDirection attackDirection, Engagement potentialEngagement, GameObject panel, GameObject container) {
		foreach (MutatorDisplayData mutator in BuildMutatorList(potentialEngagement, attackDirection)) {
			if (mutator.name == "") continue;

			string message = $"{mutator.name.RichTextTags_TMP(bold: true)}";
			if (mutator.description != "") {
				message += $"\n{mutator.description.RichTextTags_TMP(italics: true)}";
			}
			CreateAndSet(message, panel, container);
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

	private IEnumerable<MutatorDisplayData> BuildMutatorList(Engagement potentialEngagement, Attack.AttackDirection attackDirection) {
		List<MutatorDisplayData> mutators = new List<MutatorDisplayData>();

		foreach (Attack attack in potentialEngagement.GetAttacks(attackDirection)) {
			foreach (MutatorDisplayData mdd in attack.attackMutators) {
				mutators.Add(mdd);
			}			
		}
		foreach (Attack oppositeAttack in potentialEngagement.GetAttacks(Attack.OppositeDirection(attackDirection))) {
			foreach (MutatorDisplayData mdd in oppositeAttack.defenseMutators) {
				mutators.Add(mdd);
			}			
		}

		foreach (MutatorDisplayData mut in mutators.Distinct()) {
			yield return mut;
		}
	}

	private IEnumerable<MutatorDisplayData> AllMutators(Engagement potentialEngagement) {
		foreach (var mut in BuildMutatorList(potentialEngagement, Attack.AttackDirection.Normal)) yield return mut;
		foreach (var mut in BuildMutatorList(potentialEngagement, Attack.AttackDirection.Counter)) yield return mut;
	}
}