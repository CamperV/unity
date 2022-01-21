using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LaunchesConfirmDialog : MonoBehaviour
{
    [HideInInspector] public Button button;

    // use this to pause certain things when in confirmation mode
    public EventManager eventManager;

    private Button.ButtonClickedEvent confirmCallback;
    [SerializeField] private ConfirmDialog dialogPrefab;
    private ConfirmDialog dialog;

    public string displayMessage;   // set in inspector

    void Awake() {
        button = GetComponent<Button>();

        // this copies the callback before we add LaunchConfirm to it
        confirmCallback = button.onClick;
    }

    void Start() {
        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(LaunchConfirm);
    }

    private void OnDialogEnable() {
        eventManager.DisablePlayerInput();
        eventManager.EnableMenuInput();
        eventManager.menuInputController.RightMouseClickEvent += _ => {
            Destroy(dialog.gameObject);
            OnDialogDestroy();
        };
    }

    private void OnDialogDestroy() {
        eventManager.EnablePlayerInput();
        eventManager.DisableMenuInput();
    }

    private void LaunchConfirm() => StartCoroutine( ConfirmListener() );

    private IEnumerator ConfirmListener() {
        Canvas canvas = GetComponentInParent<Canvas>();
        dialog = Instantiate(dialogPrefab, canvas.transform);
        dialog.SetMessage(displayMessage);
        OnDialogEnable();

        yield return new WaitUntil(() => dialog.cancelled || dialog.confirmed);

        if (dialog.cancelled) {

        // 
        } else if (dialog.confirmed) {
            confirmCallback.Invoke();
        }

        // either way:
        Destroy(dialog.gameObject);
        OnDialogDestroy();
    }
}