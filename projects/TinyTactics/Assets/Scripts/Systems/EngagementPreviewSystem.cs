using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Extensions;
using UnityEngine.EventSystems;
using UnityEngine.Events;

interface IEngagementPreviewer {
    void EnablePreview(Engagement engagement);
    void DisablePreview(Engagement engagement);
}

public class EngagementPreviewSystem : MonoBehaviour
{
    // this class exists to be an active GameObject in the scene,
    // to have children register themselves to various flags
    // this allows for nice, separable components
    public static EngagementPreviewSystem inst = null; // enforces singleton behavior

    public Engagement currentEngagement;
	
    void Awake() {
 		if (inst == null) {
			inst = this;
		} else if (inst != this) {
			Destroy(gameObject);
		}
    }

	[Serializable] public class EngagementPreviewEvent : UnityEvent<Engagement>{}
	public EngagementPreviewEvent EnablePreviewEvent;
	public EngagementPreviewEvent DisablePreviewEvent;

    public void EnablePreview(Engagement _engagement) {
        currentEngagement = _engagement;
        EnablePreviewEvent?.Invoke(currentEngagement);
    }

    public void DisablePreview(Engagement _engagement) {
        // Disable the preview of a given engagement
        if (_engagement != null) {
            DisablePreviewEvent?.Invoke(_engagement);

        // use the currently stored, if applicable
        } else if (currentEngagement != null) {
            DisablePreviewEvent?.Invoke(currentEngagement);
        }

        // otherwise do nothing
    }
}