using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public sealed class GameManager : MonoBehaviour
{
	public static GameManager inst = null; // enforces singleton behavior
	public Campaign activeCampaign;
	
    void Awake() {
        // only allow one GameManager to exist at any time
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