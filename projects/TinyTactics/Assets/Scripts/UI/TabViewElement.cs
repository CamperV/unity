using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Extensions;
using TMPro;

[RequireComponent(typeof(Button))]
public class TabViewElement : MonoBehaviour
{
    [SerializeField] private Image myBackground;
    private Color originalColor;

    [SerializeField] private GameObject linkedPanel;
    [SerializeField] private Image linkedPanelBackgroundImage;

    [SerializeField] private Color borderColor;
    
    void Awake() {
        originalColor = myBackground.color;
    }

    public void ActivateLinkedPanel() {
        linkedPanel.SetActive(true);
        myBackground.color = linkedPanelBackgroundImage.color;
    }

    public void DeactivateLinkedPanel() {
        linkedPanel.SetActive(false);
        myBackground.color = originalColor;
    }

    public void Dim() {
        GetComponent<Image>().color = Color.black;
        GetComponentInChildren<TextMeshProUGUI>().color = (0.75f*Color.white).WithAlpha(1f);
    }

    public void Undim() {
        GetComponent<Image>().color = borderColor;
        GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
    }
}
