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

    [SerializeField] private GridEntityMap gridEntityMap;
    [SerializeField] private BattleMap battleMap;
    [SerializeField] private PlayerUnitController playerUnitController;
    [SerializeField] private EnemyUnitController enemyUnitController;

    void Awake() {
        gridEntityMap = GetComponent<GridEntityMap>();
        battleMap = GetComponentInChildren<BattleMap>();
        playerUnitController = GetComponentInChildren<PlayerUnitController>();
        enemyUnitController = GetComponentInChildren<EnemyUnitController>();
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
