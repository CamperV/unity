using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class OnClickToggleActive : MonoBehaviour
{
    [HideInInspector] public Button button;
    public GameObject toggleObject;

    // for IResettableToggle
    public EventManager eventManager;

    void Awake() => button = GetComponent<Button>();

    void OnEnable() {
        button.onClick.AddListener(ToggleObject);

        foreach (var toggle in GetComponents<IResettableToggle>()) {
            eventManager.menuInputController.RightMouseClickEvent += toggle.ResetToggle;
        }
    }

    void OnDisable() {
        button.onClick.RemoveListener(ToggleObject);

        foreach (var toggle in GetComponents<IResettableToggle>()) {
            eventManager.menuInputController.RightMouseClickEvent -= toggle.ResetToggle;
        }
    }

    private void ToggleObject() {
        toggleObject.SetActive( !toggleObject.activeInHierarchy );
    }
}