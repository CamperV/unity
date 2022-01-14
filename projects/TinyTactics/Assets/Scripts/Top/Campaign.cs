using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// this object is created in the Main Menu, and enables persistent data storage between levels
// this is the thing that will hold information such as "Current Units" and their states
public sealed class Campaign : MonoBehaviour
{
	public static Campaign active = null;

    public List<PlayerUnit> unitRoster;

	public string[] levelSequence;	// set in inspector via prefab flow

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

	public void InsertUnitsInBattle() {}

	public void BeginLevelSequence() => StartCoroutine(_BeginLevelSequence());

	private bool doneFlag = false;
	private IEnumerator _BeginLevelSequence() {
		foreach (string levelName in levelSequence) {
			LevelLoader.inst.LoadLevel(levelName);

			
			yield return new WaitUntil(() => doneFlag == true);
		}
	}
}