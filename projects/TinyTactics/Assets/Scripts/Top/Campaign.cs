using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// this object is created in the Main Menu, and enables persistent data storage between levels
// this is the thing that will hold information such as "Current Units" and their states
[RequireComponent(typeof(LevelLoader))]
public sealed class Campaign : MonoBehaviour
{
	public static Campaign active = null;

	private Dictionary<Guid, CampaignUnitGenerator.CampaignUnitData> unitRoster;
    public List<CampaignUnitGenerator.CampaignUnitData> Units => unitRoster.Values.ToList();

	public string[] levelSequence;	// set in inspector via prefab flow
	private LevelLoader levelLoader;
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

		levelLoader = GetComponent<LevelLoader>();
		unitRoster = new Dictionary<Guid, CampaignUnitGenerator.CampaignUnitData>();
	}

	public void EnlistUnit(CampaignUnitGenerator.CampaignUnitData unitData) => unitRoster[unitData.ID] = unitData;
	public CampaignUnitGenerator.CampaignUnitData UnitByID(Guid id) => unitRoster[id];
	public void SerializeUnit(CampaignUnitGenerator.CampaignUnitData unitData) => unitRoster[unitData.ID] = unitData;

	public void BeginLevelSequence() {
		StartCoroutine( LevelSequence() );
	}

	private IEnumerator LevelSequence() {
		foreach (string levelName in levelSequence) {
			// this will spin until the LevelLoader is finished
			yield return levelLoader.LoadLevelAsync(levelName);

			// now that we've loaded:
			Battle currentBattleInScene = GameObject.Find("Battle").GetComponent<Battle>();
			currentBattleInScene.ImportCampaignData(Units);
			waitForBootstrapper = false;

			// once the battle starts, we spin here until the battle resolves
			// when the battle resolves, the user has the choice to continue to the next level, or return to the main menu
			// loading the next level is signaled by CampaignBoostrapper
			yield return new WaitUntil(() => waitForBootstrapper == true);
			waitForBootstrapper = false;

			// if you're the last level, skip this step. Otherwise, go to the level up panel!
			yield return levelLoader.LoadLevelAsync("Interstitial");

			// loading the next level is signaled by CampaignBoostrapper
			yield return new WaitUntil(() => waitForBootstrapper == true);
		}

		Debug.Log($"You've finished the campaign! Show global stats");
		levelLoader.ReturnToMainMenu();
		Destroy(gameObject);
	}
}