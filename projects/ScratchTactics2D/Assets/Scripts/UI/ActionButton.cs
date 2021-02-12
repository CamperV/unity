using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Extensions;

public class ActionButton : UIElement,
                            IPointerUpHandler, IPointerDownHandler, IPointerClickHandler,
                            IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public Image image;

    private Action callbackAction;

    private bool _active = false;
    public bool active {
        get => _active;
        set {
            _active = value;
            UpdateTint( (value) ? 1.0f : 0.5f );
        }
    }

    private bool pulse = false;

    void Awake() {
        image = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (!active) return;
        callbackAction?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (!active) return;
        transform.localScale *= 1.2f;
    }
    public void OnPointerDown(PointerEventData eventData) {
        if (!active) return;
        image.color = image.color.WithTint(0.5f);
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (!active) return;
        image.color = image.color.WithTint(1.0f);
        transform.localScale = Vector3.one;
    }
    public void OnPointerUp(PointerEventData eventData) { 
        if (!active) return;
        image.color = image.color.WithTint(1.0f);
    }

    // callbacks assigned to these buttons must have no arguments or return values
    public void BindCallback(Action action) {
        callbackAction = action;
    }

    public void UpdateTint(float tint) {
        image.color = image.color.WithTint(tint);
    }

    public override void UpdateTransparency(float alpha) {
        image.color = image.color.WithAlpha(alpha);
    }

    // copy yourself, and scale/fade out, then destroy, and do it again
	public IEnumerator Pulse(float fixedTime) {
		animationStack++;
		//
        
        Vector3 pos = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1f);
		Image copy = Instantiate(GetComponent<Image>(), pos, Quaternion.identity) as Image;
        copy.transform.SetParent(transform);
        copy.raycastTarget = false;

		float timeRatio = 0.0f;
		while (timeRatio < 1.0f) {
			timeRatio += (Time.deltaTime / fixedTime);
            copy.color = copy.color.WithAlpha(1.0f - timeRatio);
			copy.transform.localScale = new Vector3(1f + timeRatio*0.5f, 1f + timeRatio*0.5f, 0);
			yield return null;
		}

		Destroy(copy.gameObject);
        //
		animationStack--;
	}

    public IEnumerator StartInfinitePulse() {
        pulse = true;
        while (pulse) {
            StartCoroutine(Pulse(0.35f));
            yield return new WaitForSeconds(1.0f);
        }
    }

    public void StopInfinitePulse() {
        pulse = false;
    }
}
