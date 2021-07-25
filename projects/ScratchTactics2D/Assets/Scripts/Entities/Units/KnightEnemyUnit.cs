using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KnightEnemyUnit : EnemyUnit
{
    // default unit stats
    public static UnitStats defaultStats {
        get {
            return new UnitStats {
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

    private UnitStats _unitStats;
    public override UnitStats unitStats {
        get => _unitStats ?? KnightEnemyUnit.defaultStats;
        set => _unitStats = value;
    }
}
