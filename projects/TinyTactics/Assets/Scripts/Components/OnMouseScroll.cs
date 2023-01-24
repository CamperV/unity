using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class OnMouseScroll : MonoBehaviour
{
    public UnityEvent OnScrollUp;
    public UnityEvent OnScrollDown;

	// ZOOM WHEEL HARDWARE SPECIFIC
	private readonly float _scrollTick = 120f;

    void Update() {
        Vector2 scrollValue = Mouse.current.scroll.ReadValue() / _scrollTick;
		if (scrollValue.y > 0)
            OnScrollUp?.Invoke();
		if (scrollValue.y < 0)
            OnScrollDown?.Invoke();
    }
}