using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitTab : MonoBehaviour
{
    [HideInInspector] public CampaignUnitGenerator.CampaignUnitData tabUnitData;
    [SerializeField] private DraftedUnitListing unitListing;
    [SerializeField] private GameObject untouchedIndicator;
    //
    [SerializeField] private Button selectButton;
    public UnitLevelUpPanel linkedPanel;

    [HideInInspector] public UnitLeveler parentUnitLeveler;

    void Awake() {
        parentUnitLeveler = GetComponentInParent<UnitLeveler>();
    }

    void Update() {
        untouchedIndicator.GetComponent<UIBobber>().MoveAnchorOffset(transform.position, 0.5f*Vector3.down);
    }

    public void CreateLinkedPanel(Transform anchor) {
        linkedPanel = Instantiate(linkedPanel, anchor);
        linkedPanel.gameObject.SetActive(false);

        selectButton.onClick.AddListener(() => parentUnitLeveler.SelectTab(this));
    }

    public void SetUnitInfo(CampaignUnitGenerator.CampaignUnitData unitData) {
        tabUnitData = unitData;
        unitListing.SetUnitInfo(unitData);
    }

    public void Select() {
        untouchedIndicator.SetActive(false);
        //
        WakeUpTab();
        linkedPanel.gameObject.SetActive(true);
    }

    public void Deselect() {
        DimTab();
        linkedPanel.gameObject.SetActive(false);
    }

    private void WakeUpTab() => GetComponentInChildren<CanvasGroup>().alpha = 1f;
    private void DimTab() => GetComponentInChildren<CanvasGroup>().alpha = 0.33f;
}
