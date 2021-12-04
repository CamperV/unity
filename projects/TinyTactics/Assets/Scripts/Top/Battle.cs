using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(GridEntityMap))]
public class Battle : MonoBehaviour
{
    [SerializeField] private PlayerInputController inputController;

    private GridEntityMap gridEntityMap;
    private BattleMap battleMap;
    private PlayerUnitController playerUnitController;

    void Awake() {
        battleMap = GetComponentInChildren<BattleMap>();
        playerUnitController = GetComponentInChildren<PlayerUnitController>();
        gridEntityMap = GetComponent<GridEntityMap>();
    }

    void Start() {
        // register all events
        inputController.MousePositionEvent += battleMap.CheckMouseOver;

        inputController.LeftMouseClickEvent += battleMap.CheckLeftMouseClick;

        inputController.RightMouseClickEvent += battleMap.CheckRightMouseClick;
        inputController.RightMouseClickEvent += _ => playerUnitController.ClearInteraction();

        battleMap.InteractEvent += playerUnitController.ContextualInteractAt;
    }
}
