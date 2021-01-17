using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	private Text currentPhaseText;
	private Text currentTurnText;

    private GameObject engagementPreview;
	
    void Awake() {
        currentPhaseText = GameObject.Find("CurrentPhaseText").GetComponent<Text>();
		currentTurnText = GameObject.Find("CurrentTurnText").GetComponent<Text>();

        engagementPreview = GameObject.Find("EngagementPreview");
        engagementPreview.gameObject.SetActive(false);
    }

    public void SetPhaseText(string text) {
		  currentPhaseText.text = text;        
    }
	
    public void SetTurnText(string text) {
        currentTurnText.text = text;        
    }

	public void DisplayEngagementPreview(Engagement engagement) {
		// display the hit percentages for the engagement's actors
        engagementPreview.gameObject.SetActive(true);
	}

    public void HideEngagementPreview() {
        engagementPreview.gameObject.SetActive(false);
    }
}