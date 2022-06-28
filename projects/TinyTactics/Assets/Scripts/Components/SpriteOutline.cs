using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Unit))]
public class SpriteOutline : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static float standardThickness = 0.025f;
    [SerializeField] private float thickness;
    [SerializeField] private Color color;

    public enum SelectionMode {
        PointerEvent,
        GridEvent
    }
    [SerializeField] private SelectionMode selectionMode;

    private bool currentValue = false;

    // assign these in inspector
    [SerializeField] private Renderer renderer;
    [SerializeField] private Material material;

    void Awake() {
		renderer.material = material;
        renderer.material.SetFloat("_Thickness", 0f);
        renderer.material.SetColor("_Color", color);

        if (thickness == 0) thickness = standardThickness;
    }

    void Start() {
        // if this isn't mouse-driven, make it battleMap-event driven instead
        if (selectionMode == SelectionMode.GridEvent) {
            GetComponent<Unit>().battleMap.GridMouseOverEvent += CheckGridPosition;
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (selectionMode == SelectionMode.PointerEvent) SetOutline(true);
    }
    public void OnPointerExit(PointerEventData eventData) {
        if (selectionMode == SelectionMode.PointerEvent) SetOutline(false);
    }

    // used by external classes to set, i.e. Unit.OnSelect()
    public void SetOutline(bool value) {
        if (value != currentValue) {
            renderer.material.SetFloat("_Thickness", (value) ? thickness : 0f);
            currentValue = value;
        }
    }

    private void CheckGridPosition(GridPosition gp) => SetOutline(gp == GetComponent<Unit>().gridPosition);
}
