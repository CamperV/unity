using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Extensions;

public class ActionMenu : UnitUIElement
{
    private Dictionary<string, ActionButton> options;
    private Dictionary<int, string> order;
    private Dictionary<string, bool> optionAvailability { get => parentUI.boundUnit.optionAvailability; }

    public ActionButton actionButtonPrefab;

    void Awake() {
        options = new Dictionary<string, ActionButton>();
        order = new Dictionary<int, string>();

        Sprite moveSprite   = ResourceLoader.GetSprite("move_icon");
        Sprite attackSprite = ResourceLoader.GetSprite("sword_icon");
        Sprite waitSprite   = ResourceLoader.GetSprite("wait_icon");
        Sprite cancelSprite = ResourceLoader.GetSprite("cancel_icon");
        AddButton("Move", moveSprite, 3);
        AddButton("Attack", attackSprite, 2);
        AddButton("Wait", waitSprite, 1);
        AddButton("Cancel", cancelSprite, 0);
    }

    void Start() {
        // reposition center of menu
        transform.localScale *= 0.25f;
        transform.position -= new Vector3(0, parentUI.boundUnit.spriteHeight*0.35f, 0);

        // linearly position menu elements
        for (int i = 0; i < order.Count; i++) {
            string opt = order[i];
            var actionButton = options[opt];
            //
            float ci = i - ( (order.Count-1) / 2.0f);
            actionButton.transform.position += new Vector3(ci*(actionButton.spriteWidth*1.1f), 0, 0);
        }
    }

    public override void UpdateTransparency(float alpha) {
        foreach (ActionButton button in options.Values) {
            button.UpdateTransparency(alpha);
        }
    }

    private void AddButton(string name, Sprite sprite, int ord) {
        ActionButton button = ActionButton.Spawn(transform, actionButtonPrefab, sprite);
        options[name] = button;
        order[ord] = name;
    }

    public void BindCallbacks(Dictionary<string, Action> callbacks) {
        foreach (string name in callbacks.Keys) {
            options[name].BindCallback(callbacks[name]);
        }
    }

    public void Display() {
        gameObject.SetActive(true);

        StartCoroutine(FadeUpToFull(standardFadeTime));
        StartCoroutine(ExecuteAfterAnimating(() => {
            // update each ActionButton's active state
            options.Keys.ToList().ForEach(it => options[it].active = optionAvailability.GetValueOtherwise(it, true));
        }));
    }
    public void ClearDisplay() {
        options.Values.ToList().ForEach(it => it.active = false);

        StartCoroutine(FadeDown(standardFadeTime));
        StartCoroutine(ExecuteAfterAnimating(() => {
            gameObject.SetActive(false);
        }));
    }
}
