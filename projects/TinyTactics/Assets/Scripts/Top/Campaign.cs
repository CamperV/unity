using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// this object is created in the Main Menu, and enables persistent data storage between levels
// this is the thing that will hold information such as "Current Units" and their states
public sealed class Campaign : MonoBehaviour
{
	public static bool isActive => CampaignManager.inst != null;
	public static Campaign active => isActive ? CampaignManager.inst.activeCampaign : null;

    public List<PlayerUnit> unitRoster;
	public string[] levelSequence;	// set in inspector via prefab flow

	void Awake() {
		DontDestroyOnLoad(gameObject);

		unitRoster = new List<PlayerUnit>();
	}

	public void EnlistUnit(PlayerUnit unit) => unitRoster.Add(unit);

	public void BeginLevelSequence() => StartCoroutine( LevelSequence() );

	private IEnumerator LevelSequence() {
		foreach (string levelName in levelSequence) {
			// this will spin until the LevelLoader is finished
			yield return GetComponent<LevelLoader>().LoadLevelAsync(levelName);

			// now that we've loaded:
			Battle currentBattleInScene = GameObject.Find("Battle").GetComponent<Battle>();
			//
			currentBattleInScene.ImportCampaignData(unitRoster);
			currentBattleInScene.BattleStartEvent += () => Debug.Log("woo");

			bool battleOverFlag = false;
			bool victoryClosure = false;
			currentBattleInScene.ConditionalBattleEndEvent += pv => {
				battleOverFlag = true;
				victoryClosure = pv;
			};
			yield return new WaitUntil(() => battleOverFlag == true);

			Debug.Log($"Campaign {this} has seen the battle end. Victoriously? {victoryClosure}");
			while (true) yield return null;
		}
	}
}