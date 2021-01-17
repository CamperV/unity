using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	// singleton
	public static UIManager inst = null; // enforces singleton behavior

	private Text currentPhaseText;
	private Text currentTurnText;

    public EngagementPreview engagementPreview;
    public EngagementPreview engagementPreviewPrefab;
	
    void Awake() {
        // only allow one UIManager to exist at any time
		// & don't kill when reloading a Scene
 		if (inst == null) {
			inst = this;
		} else if (inst != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);

        currentPhaseText = GameObject.Find("CurrentPhaseText").GetComponent<Text>();
		currentTurnText = GameObject.Find("CurrentTurnText").GetComponent<Text>();
    }

    public void SetPhaseText(string text) {
		  currentPhaseText.text = text;        
    }
	
    public void SetTurnText(string text) {
        currentTurnText.text = text;        
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
}