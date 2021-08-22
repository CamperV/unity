using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArcherEnemyUnit : EnemyUnit
{
    // default unit stats
    public static UnitState defaultState {
        get {
            return new UnitState {
                unitTag = "ArcherEnemyUnit",

                VITALITY  = 10,
                STRENGTH  = 3,
                DEXTERITY = 3,
                REFLEX    = 1,
                MOVE      = 4,

                inventoryPool = new List<Equipment>{
                    new Bow()
                }
            }.DeriveRemaining();
        }
    }

    private UnitState _unitState;
    public override UnitState unitState {
        get => _unitState ?? ArcherEnemyUnit.defaultState;
        set => _unitState = value;
    }
}
