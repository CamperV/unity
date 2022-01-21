using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class ConfirmDialog : MonoBehaviour
{
    // set in inspector
    public TextMeshProUGUI messageValue;

    public bool confirmed;
    public bool cancelled;

    void Awake() {
        confirmed = false;
        cancelled = false;
    }

    public void SetMessage(string message) {
        messageValue.SetText(message);
    }

    public void Confirm() => confirmed = true;
    public void Cancel() => cancelled = true;
}