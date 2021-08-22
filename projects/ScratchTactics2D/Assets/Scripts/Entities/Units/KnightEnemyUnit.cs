using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KnightEnemyUnit : EnemyUnit
{
    // default unit stats
    public static UnitState defaultState {
        get {
            return new UnitState {
                unitTag = "KnightEnemyUnit",

                VITALITY  = 20,
                STRENGTH  = 1,
                DEXTERITY = 1,
                REFLEX    = 3,
                MOVE      = 5,

                inventoryPool = new List<Equipment>{
                    new Sword()
                }
            }.DeriveRemaining();
        }
    }

    private UnitState _unitState;
    public override UnitState unitState {
        get => _unitState ?? KnightEnemyUnit.defaultState;
        set => _unitState = value;
    }
}
