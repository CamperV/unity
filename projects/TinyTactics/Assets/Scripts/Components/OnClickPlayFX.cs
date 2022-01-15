using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class OnClickPlayFX : MonoBehaviour
{
    [HideInInspector] public Button button;

    // assigned in Inspector
    public bool clickFX;
    public bool confirmFX;
    public bool backFX;
    public bool specialFX;
    public bool superSpecialFX;

    void Awake() => button = GetComponent<Button>();

    void Start() {
        if (clickFX) button.onClick.AddListener(OmniSound.PlayClickFX);
        if (confirmFX) button.onClick.AddListener(OmniSound.PlayConfirmFX);
        if (backFX) button.onClick.AddListener(OmniSound.PlayBackFX);
        if (specialFX) button.onClick.AddListener(OmniSound.PlaySpecialFX);
        if (superSpecialFX) button.onClick.AddListener(OmniSound.PlaySuperSpecialFX);
    }
}