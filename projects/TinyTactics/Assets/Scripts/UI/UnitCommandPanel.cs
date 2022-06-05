using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Extensions;

public class UnitCommandPanel : MonoBehaviour
{
	[SerializeField] private GameObject unitCommandContainer;
	[SerializeField] private UnitCommandVisual unitCommandVisualPrefab;

	private Dictionary<UnitCommand, UnitCommandVisual> mapping = new Dictionary<UnitCommand, UnitCommandVisual>();
	private UnitCommandSystem boundUnitCommandSystem;

	public void SetUnitInfo(PlayerUnit unit) {
		ClearUCs();

		foreach (UnitCommand uc in unit.unitCommandSystem.Commands) {
			AddToPanel(uc, unit.unitCommandSystem);
		}

		// now... can we somehow bind functions here to uc.Activate()/uc.Deactivate() a la invocation list?
		unit.unitCommandSystem.ActivateUC += ActivateTrigger;
		unit.unitCommandSystem.DeactivateUC += DeactivateTrigger;
		unit.unitCommandSystem.FinishUC += FinishTrigger;
	}

	public void ClearUnitInfo(PlayerUnit unit) {
		// clear callbacks once you're no longer interacting with the UI
		unit.unitCommandSystem.ActivateUC -= ActivateTrigger;
		unit.unitCommandSystem.DeactivateUC -= DeactivateTrigger;
		unit.unitCommandSystem.FinishUC -= FinishTrigger;
	}

	private void ClearUCs() {
		foreach (Transform t in unitCommandContainer.transform) {
			Destroy(t.gameObject);
		}

		mapping.Clear();
	}

	private void AddToPanel(UnitCommand uc, UnitCommandSystem ucs) {
		UnitCommandVisual ucv = Instantiate(unitCommandVisualPrefab, unitCommandContainer.transform);
		ucv.SetImage(uc.sprite);
		ucv.SetName(uc.name);
		ucv.RegisterCommand(() => ucs.TryIssueCommand(uc));
		ucv.SetButtonChecker(() => ucs.IsCommandAvailable(uc));
		ucv.CheckButtonStatus();

		// ucv.GetComponent<ConfirmSpriteSwap>().enabled = uc.requiresConfirm;

		// set the mapping value so that it can be stored/retrieved for visualiztion
		mapping[uc] = ucv;
	}


	private void ActivateTrigger(UnitCommand uc) {
		mapping[uc].OnActivate();
	}

	private void DeactivateTrigger(UnitCommand uc) {
		mapping[uc].OnDeactivate();
	}

	private void FinishTrigger(UnitCommand uc) {
		// just in case your ability caused others to become balid/invalid, go ahead and refresh
		foreach (UnitCommand _uc in mapping.Keys) {
			mapping[_uc].CheckButtonStatus();
		}
		mapping[uc].SetButtonStatus(false);
	}
}