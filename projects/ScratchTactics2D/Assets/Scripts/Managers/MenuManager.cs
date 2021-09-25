using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class MenuManager : MonoBehaviour
{
	// singleton
	public static MenuManager inst = null; // enforces singleton behavior

    [HideInInspector] public ActionPane actionPane;
    public ActionPane actionPanePrefab;

    [HideInInspector] public EngagementPreview engagementPreview;
    public EngagementPreview engagementPreviewPrefab;
	
    void Awake() {
        // only allow one MenuManager to exist at any time
		// & don't kill when reloading a Scene
 		if (inst == null) {
			inst = this;
		} else if (inst != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
    }

    // for right now, keep this a PlayerUnit only situation
    public void CreateActionPane(PlayerUnit unit, Dictionary<string, Action> callbacks) {
        if (actionPane == null) {
            actionPane = ActionPane.Spawn(transform, actionPanePrefab, unit);
        } else {
            actionPane.RefreshButtons();
        }
        actionPane.BindCallbacks(callbacks);
    }

    public void DestroyCurrentActionPane() {
        if (actionPane) {
            Destroy(actionPane.gameObject);
            actionPane = null;
        }
    }

	public void CreateEngagementPreview(EngagementResults engagementResults) {
		// display the hit percentages for the engagement's actors
        if (engagementPreview == null) {
            engagementPreview = EngagementPreview.Spawn(transform, engagementPreviewPrefab, engagementResults);
        }
	}

    public void DestroyCurrentEngagementPreview() {
        if (engagementPreview) {
            Destroy(engagementPreview.gameObject);
            engagementPreview = null;
        }
    }

    public void CleanUpBattleMenus() {
        DestroyCurrentActionPane();
        DestroyCurrentEngagementPreview();
    }
}