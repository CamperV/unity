using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public sealed class UnitUIProvisioner : MonoBehaviour
{
    [SerializeField] private PlayerUnitController playerUnitController;
    [SerializeField] private EnemyUnitController enemyUnitController;

    [SerializeField] private UnitUI unitUIPrefab;
    public List<UnitUI> activeUIs;

    void Awake() {
        activeUIs = new List<UnitUI>();
    
        playerUnitController.RegisteredUnit += ProvisionUI;
        enemyUnitController.RegisteredUnit += ProvisionUI;
    }

	public void ProvisionUI(Unit unit) {
        UnitUI unitUI = Instantiate(unitUIPrefab, transform);
        unitUI.AttachTo(unit);

        activeUIs.Add(unitUI);
    }
}
