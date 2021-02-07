using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Extensions;

public class ActionPane : MonoBehaviour
{
    private Unit unit;
    private Dictionary<string, bool> optionAvailability { get => unit.optionAvailability; }

    public static ActionPane Spawn(Transform parent, ActionPane prefab, Unit unit) {
        ActionPane ap = Instantiate(prefab, parent);
        ap.unit = unit;
        ap.RefreshButtons();
        //
        return ap;
    }

    public void RefreshButtons() {
        // update each ActionButton's active state
        // removed ForEach Linq for multiline readability
        // foreach (ActionButton ab in GetComponentsInChildren<ActionButton>()) {
        //     ab.active = optionAvailability.GetValueOtherwise(ab?.gameObject.name, true);
        // }
        ButtonFromName("MoveButton").active = optionAvailability["Move"];
        ButtonFromName("AttackButton").active = optionAvailability["Attack"];
        ButtonFromName("WaitButton").active = true;
        ButtonFromName("CancelButton").active = true;
    }

    public void BindCallbacks(Dictionary<string, Action> callbacks) {
        callbacks.Keys.ToList().ForEach(it => ButtonFromName(it).BindCallback(callbacks[it]));
    }

    private ActionButton ButtonFromName(string name) {
        return transform.Find(name).gameObject.GetComponent<ActionButton>();
    }
}
