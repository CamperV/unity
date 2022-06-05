using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ConfirmSpriteSwap : MonoBehaviour
{
    [SerializeField] private Sprite confirmSprite;
    private Sprite originalSprite;

    [HideInInspector] public Button button;
    private Button.ButtonClickedEvent confirmCallback;

    void Awake() {
        button = GetComponent<Button>();

        // this copies the callback before we add LaunchConfirmOption to it
        confirmCallback = button.onClick;
    }

    void Start() {
        originalSprite = ((Image)button.targetGraphic).sprite;
    }

    // take the previous callback, store it, and replace it with a confirmation layer
    void OnEnable() {
        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(LaunchConfirmOption);
    }

    void OnDisable() {
        button.onClick = confirmCallback;
        ((Image)button.targetGraphic).sprite = originalSprite;
    }

    private void LaunchConfirmOption() {
        ((Image)button.targetGraphic).sprite = confirmSprite;

        // when you launch the confirm, replace the callback 
        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(ConfirmCallback);
    }

    public void ResetConfirm() {
        ((Image)button.targetGraphic).sprite = originalSprite;

        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(LaunchConfirmOption);
    }

    private void ConfirmCallback() {
        confirmCallback.Invoke();
        ResetConfirm();
    }
}