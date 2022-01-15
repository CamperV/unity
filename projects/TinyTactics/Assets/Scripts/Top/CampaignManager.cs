using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public sealed class CampaignManager : MonoBehaviour
{
	public static CampaignManager inst = null; // enforces singleton behavior
	public Campaign activeCampaign;
	
    void Awake() {
        // only allow one CampaignManager to exist at any time
		// & don't kill when reloading a Scene
 		if (inst == null) {
			inst = this;
		} else if (inst != this) {
			Destroy(gameObject);
		}
        
        //
        DontDestroyOnLoad(gameObject);
    }

	public void CreateNewCampaign() => activeCampaign = Instantiate(activeCampaign);
	public void StartActiveCampaign() => activeCampaign.BeginLevelSequence();
}