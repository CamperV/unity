using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Extensions;

public class UnitCommandPanel : MonoBehaviour
{
	public PlayerUnit boundUnit;

	[SerializeField] public PlayerInputController inputController;
	private Dictionary<int, Action> SlotActions;
	//
	[SerializeField] private GameObject mainUCPanel;
	[SerializeField] private GameObject defaultUCPanel;
	[SerializeField] private GameObject specialUCPanel;
	[SerializeField] private UnitCommandVisual defaultUnitCommandVisualPrefab;

	private Dictionary<UnitCommand, UnitCommandVisual> mapping = new Dictionary<UnitCommand, UnitCommandVisual>();

	// reserved slots that Main Commands can't use
	private HashSet<int> _reservedSlots = new HashSet<int>{0};

	void Awake() {
		SlotActions = new Dictionary<int, Action>();
	}

	void Start() {
		// bind the SelectSlot actions to QuickBar keys
		inputController.QuickBarSlotSelectEvent += SelectSlot;
	}

	public void SetUnitInfo(PlayerUnit unit) {
		boundUnit = unit;
		//
		ClearUCs();

		// determine slot numbers first, we need all UCVisuals to know about each other
		UnitCommand[] ucSlotOrder = new UnitCommand[10];
		//
		foreach (UnitCommand uc in unit.unitCommandSystem.Commands.Where(it => it.panelSlot != -1)) {
			if (ucSlotOrder[uc.panelSlot] == null) {
				ucSlotOrder[uc.panelSlot] = uc;
			} else {
				Debug.LogError($"Slot already taken by {ucSlotOrder[uc.panelSlot]}");
			}
		}

		// then, insert others at the minumum unoccupied slot
		foreach (UnitCommand uc in unit.unitCommandSystem.Commands.Where(it => it.panelSlot == -1)) {
			// first index of unoccupied space
			int firstIndex = ucSlotOrder.TakeWhile((it, index) => it != null || _reservedSlots.Contains(index)).Count();
			ucSlotOrder[firstIndex] = uc;
		}

		// now finally, add to panel
		// let these be activated when something is added to them
		mainUCPanel.SetActive(false);
		defaultUCPanel.SetActive(false);
		specialUCPanel.SetActive(false);
		//
		for (int s = 0; s < ucSlotOrder.Length; s++) {
			if (ucSlotOrder[s] != null) {
				AddToPanel(ucSlotOrder[s], unit.unitCommandSystem, s);
			}
		}

		// now that the UnitCommandVisuals have been created, make them noisy
		foreach (UnitCommandVisual ucv in mapping.Values) {
			ucv.RegisterCommand(unit.personalAudioFX.PlayInteractFX);
		}

		// now... can we somehow bind functions here to uc.Activate()/uc.Deactivate() a la invocation list?
		unit.unitCommandSystem.ActivateUC += ActivateTrigger;
		unit.unitCommandSystem.DeactivateUC += DeactivateTrigger;
		unit.unitCommandSystem.FinishUC += FinishTrigger;
		unit.unitCommandSystem.RevertUC += RevertTrigger;
	}

	public void ClearUnitInfo(PlayerUnit unit) {
		boundUnit = null;

		// clear callbacks once you're no longer interacting with the UI
		unit.unitCommandSystem.ActivateUC -= ActivateTrigger;
		unit.unitCommandSystem.DeactivateUC -= DeactivateTrigger;
		unit.unitCommandSystem.FinishUC -= FinishTrigger;
		unit.unitCommandSystem.RevertUC -= RevertTrigger;
	}

	private void ClearUCs() {
		foreach (Transform t in defaultUCPanel.transform) Destroy(t.gameObject);
		foreach (Transform t in mainUCPanel.transform) Destroy(t.gameObject);
		foreach (Transform t in specialUCPanel.transform) Destroy(t.gameObject);

		mapping.Clear();
		SlotActions.Clear();
	}

	private void AddToPanel(UnitCommand uc, UnitCommandSystem ucs, int slot) {
		//
		// 1) decide which panel to add to
		//
		GameObject appropriatePanel = defaultUCPanel;
		switch (uc.panelCategory) {
			case UnitCommand.PanelCategory.Main:
				appropriatePanel = mainUCPanel;
				break;
			case UnitCommand.PanelCategory.Default:
				appropriatePanel = defaultUCPanel;
				break;
			case UnitCommand.PanelCategory.Special:
				appropriatePanel = specialUCPanel;
				break;
		}

		//
		// 2) set generic info
		//
		appropriatePanel.SetActive(true);
		UnitCommandVisual ucvPrefab = (uc.unitCommandVisualPrefab == null) ? defaultUnitCommandVisualPrefab : uc.unitCommandVisualPrefab;
		UnitCommandVisual ucv = Instantiate(ucvPrefab, appropriatePanel.transform);
		ucv.SetCommandInfo(uc);

		//
		// 3) register behavior
		//
		ucv.RegisterCommand(() => ucs.TryIssueCommand(uc));
		ucv.SetButtonChecker(() => ucs.IsCommandAvailable(uc));
		ucv.CheckButtonStatus();

		//
		// 4) register limittype monitoring, and immediately call it
		//
		switch (uc.limitType) {
			case UnitCommand.LimitType.Cooldown:
				ucv.SetLimitTypeUpdater(() => ucv.SetCooldown(ucs.CommandCooldown(uc)));
				ucv.UpdateLimitType();
				break;

			case UnitCommand.LimitType.LimitedUse:
				ucv.SetLimitTypeUpdater(() => ucv.SetRemainingUses(ucs.CommandRemainingUses(uc)));
				ucv.UpdateLimitType();
				break;
		}

		//
		// 5) set the mapping value so that it can be stored/retrieved for visualiztion
		//
		mapping[uc] = ucv;
		ucv.SetSlotNumber(slot);

		// bind activation via numpad/numrow here
		SlotActions[slot] = () => ucs.TryIssueCommand(uc);

		//
		// disabled for now, janky and not really working
		// ucv.GetComponent<ConfirmSpriteSwap>().enabled = uc.requiresConfirm;
	}

	private void SelectSlot(int slot) {
		if (SlotActions.ContainsKey(slot)) SlotActions[slot].Invoke();
	}

	private void ActivateTrigger(PlayerUnit thisUnit, UnitCommand uc) {
		mapping[uc].OnActivate(thisUnit, uc);
	}

	private void DeactivateTrigger(PlayerUnit thisUnit, UnitCommand uc) {
		mapping[uc].OnDeactivate(thisUnit, uc);
	}

	private void FinishTrigger(PlayerUnit thisUnit, UnitCommand uc) {
		// just in case your ability caused others to become balid/invalid, go ahead and refresh
		foreach (UnitCommand _uc in mapping.Keys) {
			mapping[_uc].CheckButtonStatus();
			mapping[uc].UpdateLimitType();
		}
		mapping[uc].SetButtonStatus(false);
	}

	private void RevertTrigger(PlayerUnit thisUnit, UnitCommand uc) {
		mapping[uc].CheckButtonStatus();
		mapping[uc].UpdateLimitType();	
	}
}