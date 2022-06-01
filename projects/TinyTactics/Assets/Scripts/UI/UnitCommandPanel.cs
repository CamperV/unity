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
		ucv.RegisterCommand(() => ucs.TryIssueCommand(uc));
		ucv.SetButtonStatus(ucs.IsCommandAvailable(uc));

		// set the mapping value so that it can be stored/retrieved for visualiztion
		mapping[uc] = ucv;

		// // create a raw GameObject and juice it up here
		// GameObject ucVisual = new GameObject();
		// ucVisual.name = uc.name;

		// ucVisual.AddComponent<RectTransform>();
		// ucVisual.AddComponent<CanvasRenderer>();
		// Image im = ucVisual.AddComponent<Image>();
		// im.sprite = uc.sprite;

		// ucVisual.transform.parent = unitCommandContainer.transform;	
		// ucVisual.transform.localScale = Vector3.one;

		// // now attach the right functions
		// Button btn = ucVisual.AddComponent<Button>();
		// btn.onClick.AddListener(() => ucs.TryIssueCommand(uc));

		// // now, set various active... things
		// // im.color = ucs.IsCommandAvailable(uc) ? Color.white : (0.5f*Color.white).WithAlpha(1f);
		// btn.interactable = ucs.IsCommandAvailable(uc);

		// // set the mapping value so that it can be stored/retrieved for visualiztion
		// mapping[uc] = ucVisual;
	}

	private void ActivateTrigger(UnitCommand uc) {
		mapping[uc].OnActivate();
	}

	private void DeactivateTrigger(UnitCommand uc) {
		mapping[uc].OnDeactivate();
	}

	private void FinishTrigger(UnitCommand uc) {
		mapping[uc].SetButtonStatus(false);
	}
}