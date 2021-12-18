using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Extensions;
using TMPro;

public class HoldTimer : MonoBehaviour
{
    private readonly float fixedHoldTime = 1f;
    private float holdTimeElapsed = 0f;

    private Coroutine holdCoroutine;

    // assigned in inspector
    [SerializeField] private Image holdTimerVisualization;

    // other
    private Unit boundUnit;
    private Color originalHoldTimerColor;

    void Awake() {
        originalHoldTimerColor = holdTimerVisualization.color;
        boundUnit = GetComponent<Unit>();
    }

    public void StartTimer(Action OnHold, Action OnCancel) {
        holdTimeElapsed = 0f;
        //
        holdCoroutine = StartCoroutine(
            _HoldTimer(fixedHoldTime, OnHold, OnCancel)
        );
    }

    public void CancelTimer() {
        holdTimeElapsed = 0f;
        //
        if (holdCoroutine != null) StopCoroutine(holdCoroutine);

        // revert colors, etc
        holdTimerVisualization.color = originalHoldTimerColor.WithAlpha(0f);
        boundUnit.RevertColor();
    }
    
    private IEnumerator _HoldTimer(float maxTime, Action OnHold, Action OnCancel) {

        // count until you reach maxTime
        while (holdTimeElapsed < maxTime) {

            // if the GridPosition  where the MouseHold was initiated is the same, continually count up
            if (boundUnit.MouseHovering) {
                holdTimeElapsed += Time.deltaTime;

                float percentComplete = holdTimeElapsed / maxTime;
                holdTimerVisualization.fillAmount = 1.5f*percentComplete;
                holdTimerVisualization.color = Color.Lerp(new Color(0.75f, 0.75f, 0.75f, 1f).WithAlpha(0.25f), originalHoldTimerColor.WithAlpha(1f), 1.5f*percentComplete);

                boundUnit.LerpInactiveColor(percentComplete);
                yield return null;
        
            // break out here if the mouse has been moved out of the tile
            } else {
                OnCancel();
                yield break;
            }
        }

        //
        // trigger OnHold here
        //
        OnHold();

        float flourishTime = 0f;
        float flourishTotal = 0.4f;
        while (flourishTime < flourishTotal) {
            flourishTime += Time.deltaTime;
            
            float flourishComplete = flourishTime / flourishTotal;
            float flourishEaseOut = 1f - Mathf.Pow(1f - flourishComplete, 5f);
            holdTimerVisualization.color = Color.Lerp(originalHoldTimerColor.WithAlpha(1f), originalHoldTimerColor.WithAlpha(0f), flourishComplete);

            float scaler = Mathf.Lerp(1f, 1.5f, flourishEaseOut);
            holdTimerVisualization.transform.localScale = scaler*Vector3.one;
            yield return null;
        }

        // if you've made it here, you legally completed holding down the mouse in one area
        // otherwise you would have exited early
        holdTimerVisualization.color = originalHoldTimerColor.WithAlpha(0f);
        holdTimerVisualization.transform.localScale = Vector3.one;
    }
}
