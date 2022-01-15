using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// this object is created in the Main Menu, and enables persistent data storage between levels
// this is the thing that will hold information such as "Current Units" and their states
[RequireComponent(typeof(LevelLoader))]
public sealed class Campaign : MonoBehaviour
{
	public static Campaign active = null;

    public List<PlayerUnit> unitRoster;
	public string[] levelSequence;	// set in inspector via prefab flow
	public bool waitForBootstrapper = false;

	void Awake() {
        // only allow one Campaign to exist at any time
		// & don't kill when reloading a Scene
 		if (active == null) {
			active = this;
		} else if (active != this) {
			Destroy(gameObject);
		}
        
        //
		DontDestroyOnLoad(gameObject);

		unitRoster = new List<PlayerUnit>();
	}

	public void EnlistUnit(PlayerUnit unit) => unitRoster.Add(unit);

	public void BeginLevelSequence() {
		StartCoroutine( LevelSequence() );
	}

	private IEnumerator LevelSequence() {
		foreach (string levelName in levelSequence) {
			waitForBootstrapper = false;
			
			// this will spin until the LevelLoader is finished
			yield return GetComponent<LevelLoader>().LoadLevelAsync(levelName);

			// now that we've loaded:
			Battle currentBattleInScene = GameObject.Find("Battle").GetComponent<Battle>();
			currentBattleInScene.ImportCampaignData(unitRoster);

			// once the battle starts, we spin here until the battle resolves
			// when the battle resolves, the user has the choice to continue to the next level, or return to the main menu
			// loadNextLevel is signaled by CampaignBoostrapper
			yield return new WaitUntil(() => waitForBootstrapper == true);
		}

		Debug.Log($"You've finished the campaign! Show global stats");
		GetComponent<LevelLoader>().ReturnToMainMenu();
		Destroy(gameObject);
	}
}