using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EventManager : MonoBehaviour
{
    public static EventManager inst;

    public PlayerInputController inputController;
    public BattleMap battleMap;
    public PlayerUnitController playerUnitController;
    public EnemyUnitController enemyUnitController;

    void Awake() {
		// only allow one EventManager to exist at any time
		// & don't kill when reloading a Scene
 		if (inst == null) {
			inst = this;
		} else if (inst != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
    }

    void Start() {
        inputController.MousePositionEvent += battleMap.CheckMouseOver;
        inputController.LeftMouseClickEvent += battleMap.CheckLeftMouseClick;
        inputController.RightMouseClickEvent += battleMap.CheckRightMouseClick;
        inputController.RightMouseClickEvent += _ => playerUnitController.ClearInteraction();

        battleMap.InteractEvent += playerUnitController.ContextualInteractAt;
        battleMap.InteractEvent += enemyUnitController.ContextualInteractAt;
    }
}
